using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using RobotDefine;

using RobotFactory.Tasks;

namespace RobotFactory
{
    /// <summary>
    /// 虚拟机器人
    /// </summary>
    public class VirtualRobot : IDisposable
    {
        /// <summary>
        /// 机器人实时状态
        /// </summary>
        public MRStatus MRStatus { get; set; }
        /// <summary>
        /// 请求更新实时状态
        /// </summary>
        public event MrRequestStatusRefreshEventHandler OnMrRequestStatusRefresh;
        /// <summary>
        /// 当前電池電量百分比（充电前）,充电停止后会自动更新此电量
        /// </summary>
        public double Battery { get; set; }

        #region 任务

        /// <summary>
        /// 当前待处理任务
        /// </summary>
        private readonly Queue<IRobotTask> _tasks = new Queue<IRobotTask>();
        private CancellationTokenSource _cancelTokenSource;
        private Thread _watchThread;
        public int TaskCount => _tasks.Count;
        private object syncRoot = new object();
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);

        public void AddTask(IRobotTask task)
        {
            lock (syncRoot)
            {
                _tasks.Enqueue(task);
                if (_tasks.Count > 0 && _watchThread == null) //启动任务线程
                {
                    _cancelTokenSource = new CancellationTokenSource();
                    _watchThread = new Thread(TaskWorkerThread) { IsBackground = true };
                    _watchThread.Start();
                }
            }
            _waitHandle.Set(); // Signal to the thread there is data to process

        }
        /// <summary>执行与释放或重置非托管资源相关的应用程序定义的任务。</summary>
        public void Dispose()
        {
            try
            {
                _cancelTokenSource.Cancel(false);
                _watchThread.Join();
                _watchThread = null;
            }
            catch
            {
                // ignored
            }
        }
        private void TaskWorkerThread()
        {
            while (!_cancelTokenSource.IsCancellationRequested)
            {

                IRobotTask task = null;
                var rollUpdateStatus = false;
                lock (syncRoot) //锁住防止新的Task被加入
                {
                    if (_tasks.Count > 0)
                    {
                        if (!IsVirtualRobotCanWork(_tasks.First()))
                        {
                            Console.WriteLine($"[当前Robot正忙({MRStatus.MRID})] Execute:UpdateStatus");
                            //VirtualRobot状态此时不适合做任务，那么开启状态轮询线程，直到状态检查被通过。
                            UpdateStatus(); //刷新状态
                            rollUpdateStatus = true;//开启轮询
                        }
                        else
                        {
                            task = _tasks.Dequeue(); //任務出列
                            Console.WriteLine($"[{MRStatus.MRID}：开始任务->{task.Id}]");
                            task.Run(this); //運行任務
                            Console.WriteLine($"[{MRStatus.MRID}：结束任务->{task.Id}]");
                        }
                    }
                }

                if ((task != null || rollUpdateStatus) && _tasks.Count > 0)
                {
                    //Thread.Sleep(300);
                    Thread.Sleep(rollUpdateStatus?5000:100); //轮询为5秒一次
                }
                else
                {
                    _waitHandle.WaitOne();
                }

            }
        }
        /// <summary>
        /// 檢查MR狀態是否可以執行任務
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool IsVirtualRobotCanWork(IRobotTask task)
        {
            if (MRStatus.Battery < 30) //电池规则检查
            {
                return false;
            }

            return IsIdle();
        }

        #endregion

        public void UpdateStatus()
        {
            OnMrRequestStatusRefresh?.Invoke(this, new MrIdArg
            {
                MRID = MRStatus.MRID
            });
        }

        public void OnMission(bool mission)
        {
            MRStatus.IOperatorStatus = mission ? IOperatorStatus.Busy : IOperatorStatus.Idle;
            MRStatus.MissionStatus = mission ? MissionStatus.OnMission : MissionStatus.Standby;
            if (!mission)
            {
                UpdateStatus();
            }
        }
        /// <summary>
        /// 当更新最新的MRStatus事件
        /// </summary>
        /// <param name="eMrStatus"></param>
        public void OnMRStatusChange(MRStatus eMrStatus)
        {
            this.MRStatus = eMrStatus;
            if (this.TaskCount > 0) //当前还有任务。继续执行
            {
                _waitHandle.Set(); // Signal to the thread there is data to process
            }
        }
        /// <summary>
        /// 是否处于空闲状态
        /// </summary>
        /// <returns></returns>
        public bool IsIdle()
        {
            return  MRStatus.IOperatorStatus == IOperatorStatus.Idle && MRStatus.MissionStatus == MissionStatus.Standby;
        }
    }
}
