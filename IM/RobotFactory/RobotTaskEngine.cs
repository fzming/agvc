﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using AgvcWorkFactory.Interfaces;
using AgvcWorkFactory.Tasks;
using Messages.Transfers;
using Messages.Transfers.Core;
using Utility.Extensions;

namespace AgvcWorkFactory
{
    /// <summary>
    ///     Robot 任务引擎
    /// </summary>
    public sealed class RobotTaskEngine : IRobotTaskEngine
    {
        private static IEnumerable<Type> _taskTypes;
        private readonly object _locker = new();
        private readonly AutoResetEvent _waitHandle = new(false);
        private CancellationTokenSource _cancelTokenSource;
        private Thread _watchThread;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="robotManager">机器人管理器</param>
        /// <param name="agvReporter">Agv设备状态上报监控对象</param>
        public RobotTaskEngine(IVirtualRobotManager robotManager, IAgvReporter agvReporter)
        {
            RobotManager = robotManager;
            AgvReporter = agvReporter;
            Console.WriteLine("AgvReporter->" + AgvReporter.GetHashCode());
        }

        private IVirtualRobotManager RobotManager { get; }
        private IAgvReporter AgvReporter { get; }

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
        ///     任务队列
        /// </summary>
        public ConcurrentQueue<IRobotTask> TaskQueue { get; set; } = new();

        /// <summary>
        ///     Accept MES消息 Transfer Request(MES->AGVC)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="MRID">指定MR接受任务</param>
        public void AcceptMessage(IMessage message, string MRID)
        {
            switch (message)
            {
                // Transfer Request
                case Tx501i tx501I:
                {
                    var pathType = GetTaskPathType(tx501I);
                    var robotTask = CreateRobotTask(pathType);
                    robotTask.MRID = MRID;
                    //执行两条搬运指令
                    robotTask.AddTrxMessage(message);
                    // robotTask.AddTrxMessage(message);

                    AddTask(robotTask);
                    break;
                }
            }
        }

        /// <summary>
        ///     Accept 用户任务指令,一般情况下:用户指令由WebApi调用传输
        /// </summary>
        public void AcceptUserTask(ITask userTask)
        {
            var pathType = GetTaskPathType(userTask);
            var robotTask = CreateRobotTask(pathType);
            if (robotTask != null) AddTask(robotTask);
        }

        #region IDisposable

        /// <summary>执行与释放或重置非托管资源相关的应用程序定义的任务。</summary>
        public void Dispose()
        {
            Stop();
        }

        #endregion

        private TaskPathType GetTaskPathType(Tx501i tx501I)
        {
            //从Message中分析出PathType,
            return TaskPathType.StockToEQP;
        }

        /// <summary>
        ///     获取当前任务的路径类型
        /// </summary>
        /// <param name="userTask"></param>
        /// <returns></returns>
        private TaskPathType GetTaskPathType(ITask userTask)
        {
            //从UserTask分析
            return TaskPathType.StockToEQP;
        }

        /// <summary>
        ///     将任务增加到待分配队列
        /// </summary>
        /// <param name="robotTask"></param>
        private void AddTask(IRobotTask robotTask)
        {
            //锁:防止TaskQueque出列时,同时又有新的任务加入到队列造成冲突
            lock (_locker)
            {
                TaskQueue.Enqueue(robotTask);
                Console.WriteLine($"New Task:{TaskQueue.Count} ID:{robotTask.Id}");
                _waitHandle.Set(); // 指示任务分配线程取消阻塞状态,开始执行分配操作,
            }
        }

        #region 创建RobotTask

        /// <summary>
        ///     根据路径类型创建一个对应的机器人任务
        /// </summary>
        /// <param name="pathType"></param>
        /// <returns></returns>
        private IRobotTask CreateRobotTask(TaskPathType pathType)
        {
            var taskType = pathType.GetAttribute<TaskTypeAttribute>().TaskType;
            var type = TaskTypes.FirstOrDefault(p => p.GetCustomAttribute<TaskTypeAttribute>().TaskType == taskType);
            if (type != null)
            {
                var task = Activator.CreateInstance(type) as IRobotTask;
                task.TaskType = taskType;
                task.PathType = pathType;
                task.SetAgvReporter(AgvReporter);
                return task;
            }

            return null;
        }

        #endregion

        #region 任务分配线程

        /// <summary>
        ///     启动线程工作
        /// </summary>
        public void Start()
        {
            Console.WriteLine("[RobotTaskEngine->Start]");
            _cancelTokenSource = new CancellationTokenSource();
            _watchThread = new Thread(QueueWatchThread) {IsBackground = true};
            _watchThread.Start();
        }

        /// <summary>
        ///     停止线程工作
        /// </summary>
        public void Stop()
        {
            try
            {
                Console.WriteLine("[RobotTaskEngine->Stopping]");
                _cancelTokenSource.Cancel(false);
                _watchThread.Join();
                _watchThread = null;
                _waitHandle.Close(); //销毁原子信号锁
                Console.WriteLine("[RobotTaskEngine->Stopped]");
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        ///     任务分配实际工作线程
        /// </summary>
        private void QueueWatchThread()
        {
            while (!_cancelTokenSource.IsCancellationRequested)
            {
                lock (_locker) //防止在查找Robot时有新的Task被加入
                {
                    if (TaskQueue.Count > 0)
                    {
                        var forDequeueTask = TaskQueue.First();
                        VirtualRobot idleRobot;
                        if (!string.IsNullOrEmpty(forDequeueTask.MRID)) //如果是指定了MRID
                        {
                            idleRobot = RobotManager.FindRobot(forDequeueTask.MRID);
                        }
                        else //随机查找空闲可执行任务的机器人
                        {
                            var idleRobots = RobotManager.FindIdleRobots();
                            idleRobot = idleRobots.TakeOne();
                        }

                        if (idleRobot != null)
                            if (TaskQueue.TryDequeue(out var task)) //出队
                                idleRobot.AddTask(task); //为机器人添加新任务
                    }
                }

                if (RobotManager.GetAllVirtualRobots().Any() && TaskQueue.Count > 0)
                {
                    // Console.WriteLine("当前任务剩余：" + TaskQueue.Count);
                    Thread.Sleep(1000); // 当有任务时：每隔1秒检查空闲机器人
                }
                else
                {
                    Console.WriteLine("当前无任务指派[阻塞]");
                    _waitHandle.WaitOne(); //阻塞
                }
            }
        }

        #endregion
    }
}