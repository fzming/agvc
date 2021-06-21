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
        private static IEnumerable<Type> _taskTypes;
        private CancellationTokenSource _cancelTokenSource;
        private Thread _watchThread;
        private object syncRoot = new object();
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private static IEnumerable<Type> TaskTypes
        {
            get
            {
                _taskTypes ??= Assembly.GetAssembly(typeof(RobotTask)).GetTypes()
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
        public void TransferMessage(IMessage message)
        {

            if (message is Tx501i tx501I) // Transfer Request
            {

                var robotTask = CreateRobotTask(GetTaskTypeFromMessage(tx501I));
                robotTask.TransferRequestMessage = message;

                //将任务加入到队列
                TaskQueue.Enqueue(robotTask);

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
                VirtualRobotManager.Dispose();
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

                lock (syncRoot)
                {
                    if (TaskQueue.Count > 0)
                    {
                        //查找空闲可执行任务的机器人
                        var robot = VirtualRobotManager.FindIdleRobot();

                        if (robot != null)
                        {
                            var task = TaskQueue.Dequeue(); //出队
                            robot.AddTask(task); //为机器人添加新任务
                        }
                    }
                }

                if (TaskQueue.Count > 0)
                {
                    Console.WriteLine("当前任务剩余：" + TaskQueue.Count);
                    Thread.Sleep(500); // 当有任务时：每隔500毫秒检查空闲机器人

                }
                else
                {
                    Console.WriteLine("当前无任务指派");
                    _waitHandle.WaitOne();//阻塞
                }


            }
        }

        #endregion

        #region 创建RobotTask

        private RobotTaskType GetTaskTypeFromMessage(Tx501i tx501I)
        {
            //暂定为Stock2EQP
            return RobotTaskType.Stock2EQP;
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