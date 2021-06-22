using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using Messages.Transfers;
using Messages.Transfers.Core;

namespace RobotFactory.Tasks
{
    /// <summary>
    /// Robot 任务引擎
    /// </summary>
    public sealed class RobotTaskEngine : IDisposable
    {
        private VirtualRobotManager RobotManager { get; }
        private static IEnumerable<Type> _taskTypes;
        private CancellationTokenSource _cancelTokenSource;
        private Thread _watchThread;
        private readonly object _syncRoot = new object();
        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public RobotTaskEngine(VirtualRobotManager robotManager)
        {
            RobotManager = robotManager;
        }

        private static IEnumerable<Type> TaskTypes
        {
            get
            {
                _taskTypes ??= Assembly.GetAssembly(typeof(AbstractRobotTask)).GetTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(IRobotTask)) && t.IsAbstract == false);
                return _taskTypes;
            }
        }
        /// <summary>
        /// 任务队列
        /// </summary>
        public Queue<IRobotTask> TaskQueue { get; set; } = new();

        /// <summary>
        /// 传输MES消息 Transfer Request
        /// </summary>
        /// <param name="message"></param>
        /// <param name="MRID">指定MR接受任务</param>
        public void TransferMessage(IMessage message, string MRID)
        {
            switch (message)
            {
                // Transfer Request
                case Tx501i tx501I:
                {
                    var robotTask = CreateRobotTask(GetTaskTypeFromMessage(tx501I));
                    robotTask.MRID = MRID;
                    //执行两条搬运指令
                    robotTask.AddTrxMessage(message);
                   // robotTask.AddTrxMessage(message);

                    AddTask(robotTask);
                    break;
                }
            }
        }

        private void AddTask(IRobotTask robotTask)
        {
            lock (_syncRoot)
            {
                //将任务加入到队列
                TaskQueue.Enqueue(robotTask);
                Console.WriteLine($"New Task:{TaskQueue.Count} ID:{robotTask.Id}");
                _waitHandle.Set(); // Signal to the thread there is data to process
            }

        }

        #region IDisposable


        /// <summary>执行与释放或重置非托管资源相关的应用程序定义的任务。</summary>
        public void Dispose()
        {
            this.Stop();
        }

        #endregion

        #region 任务分配线程

        public void Start()
        {
            Console.WriteLine("[RobotTaskEngine->Start]");
            _cancelTokenSource = new CancellationTokenSource();
            _watchThread = new Thread(new ThreadStart(QueueWatchThread)) { IsBackground = true };
            _watchThread.Start();
        }
        public void Stop()
        {
            try
            {
                Console.WriteLine("[RobotTaskEngine->Stopping]");
                _cancelTokenSource.Cancel(false);
                _watchThread.Join();
                _watchThread = null;
                _waitHandle.Close();
                Console.WriteLine("[RobotTaskEngine->Stopped]");
            }
            catch
            {
                // ignored
            }
        }

        private void QueueWatchThread()
        {
            while (!_cancelTokenSource.IsCancellationRequested)
            {

                lock (_syncRoot)
                {
                    if (TaskQueue.Count > 0)
                    {
                        //bug:如果无法正确获取MR的正确工作状态，导致每次查找出来的MR都是空闲的。
                        var forDequeueTask = TaskQueue.First();
                        VirtualRobot idleRobot;
                        if (!string.IsNullOrEmpty(forDequeueTask.MRID)) //如果是指定了MRID
                        {
                            idleRobot = RobotManager.FindRobot(forDequeueTask.MRID);
                        }
                        else  //查找空闲可执行任务的机器人
                        {
                            var idleRobots = RobotManager.FindIdleRobots();
                            idleRobot = idleRobots.FirstOrDefault();
                        }

                        Console.WriteLine($"[TaskQueue.Count={TaskQueue.Count}] Robot:{idleRobot?.MRStatus.MRID ?? "null"}");
                        if (idleRobot != null)
                        {
                            var task = TaskQueue.Dequeue(); //出队
                            idleRobot.AddTask(task); //为机器人添加新任务
                        }
                    }
                }

                if (TaskQueue.Count > 0)
                {
                    // Console.WriteLine("当前任务剩余：" + TaskQueue.Count);
                    Thread.Sleep(1000); // 当有任务时：每隔5秒检查空闲机器人

                }
                else
                {
                    Console.WriteLine("当前无任务指派[阻塞]");
                    _waitHandle.WaitOne();//阻塞
                }


            }
        }

        #endregion

        #region 创建RobotTask

        private RobotTaskType GetTaskTypeFromMessage(Tx501i tx501I)
        {
            return RobotTaskType.Transfer;
        }

        private IRobotTask CreateRobotTask(RobotTaskType taskType)
        {
            var type = TaskTypes.FirstOrDefault(p => p.GetCustomAttribute<TaskTypeAttribute>().TaskType == taskType);
            if (type != null)
            {
                return Activator.CreateInstance(type) as IRobotTask;
            }

            return null;
        }

        #endregion
    }
}