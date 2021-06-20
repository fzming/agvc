using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Messages.Transfers;
using Messages.Transfers.Core;

namespace RobotFactory
{
    /// <summary>
    /// Robot 任务引擎
    /// </summary>
    public sealed class RobotTaskEngine : IDisposable
    {
        private static IEnumerable<Type> _taskTypes;
        private CancellationTokenSource _cancelTokenSource;
        private static readonly object _locker = new();
        private Thread _watchThread;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public RobotTaskEngine()
        {
             this.Start();
        }

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
            _cancelTokenSource = new CancellationTokenSource();
            _watchThread = new Thread(QueueWatchThread) { IsBackground = true };
            _watchThread.Start();
        }

        public void Stop()
        {
            try
            {
                 VirtualRobotManager.Dispose();
                _cancelTokenSource.Cancel(false);
                _watchThread.Join();
                _watchThread = null;
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
                //Console.WriteLine("队列内任务量:" + (TaskQueue.Count));//输出时间 毫秒
                lock (_locker)
                {
                    while (TaskQueue.Count > 0)
                    {
                        var robot = VirtualRobotManager.FindIdleRobot();//查找闲置机器人
                        if (robot==null)
                        {
                            return;
                        }
                        var task = TaskQueue.Dequeue(); //出队
                        robot.AddTask(task); //为机器人添加新任务
                        //
                        // Thread.Sleep(10);
                    }
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