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
        public double BeforeDockBattery { get; set; }
        public bool Docking { get; set; }
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
        /// <summary>
        /// 正处于任务执行状态
        /// </summary>
        public bool Working { get; set; }
        /// <summary>
        /// 将任务直接加入队列并等待执行
        /// </summary>
        /// <param name="task"></param>
        private void AddTaskInQueque(IRobotTask task)
        {

            Console.WriteLine($"[{MRStatus.MRID}] 添加了新任务->TaskId:{task.Id} TaskCount:{TaskCount} ");
            _tasks.Enqueue(task);
            if (_watchThread == null) //启动任务线程
            {
                _cancelTokenSource = new CancellationTokenSource();
                _watchThread = new Thread(TaskWorkerThread) { IsBackground = true };
                _watchThread.Start();
            }

            if (!Working)
            {
                Console.WriteLine($"尝试取消{MRStatus.MRID}阻塞状态");
                _waitHandle.Set(); //取消阻塞
            }

        }
        public void AddTask(IRobotTask task)
        {
            if (string.IsNullOrEmpty(task.MRID))
            {
                lock (syncRoot)
                {
                    //锁住，当MR正在做任务时，不允许新的任务加入。
                    AddTaskInQueque(task);
                }
            }
            else
            {
                AddTaskInQueque(task);
            }
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
                    if (!Working && _tasks.Count > 0) //只有在非工作状态标记下才执行
                    {
                        if (!IsRobotReadyWork(_tasks.First()))
                        {
                            //不适合做任务的原因================
                            //1.电量不足
                            //2.不处于空闲状态
                            //3.当前任务因素
                            //================================
                            RequestUpdateStatusAsync();

                            Console.WriteLine($"[{MRStatus.MRID}]开启状态轮询");

                            rollUpdateStatus = true;//开启状态轮询
                        }
                        else //注意：下列代码将长时间运行，会导致SyncRoot被长期占用
                        {
                            try
                            {
                                SetWorkingStatus(true);
                                task = _tasks.Dequeue(); //任務出列
                                Console.WriteLine($"[{MRStatus.MRID}：开始任务->{task.Id}] 剩余任务数：{_tasks.Count}");
                                task.Run(this); //運行任務
                                Console.WriteLine($"[{MRStatus.MRID}：结束任务->{task.Id}]");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                throw;
                            }
                            finally
                            {
                                SetWorkingStatus(false);
                                //任务执行完毕后，需要更新MR状态
                                RequestUpdateStatusSync();
                            }

                        }
                    }
                }

                if ((task != null || rollUpdateStatus) && _tasks.Count > 0)
                {
                    //Thread.Sleep(300);
                    Thread.Sleep(rollUpdateStatus ? 5000 : 100); //轮询为5秒一次
                }
                else
                {
                    Console.WriteLine($"[{MRStatus.MRID}] 阻塞");
                    SetWorkingStatus(false);
                    _waitHandle.WaitOne();
                }

            }
        }
        /// <summary>
        /// 檢查MR狀態是否可以執行任務
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool IsRobotReadyWork(IRobotTask task)
        {
            if (MRStatus.IOperatorStatus == IOperatorStatus.Docked) //当前正在充电
            {
                return CheckBattery();//仅检查充电是否可以完毕
            }
            return CheckBattery() && IsIdle();
        }
        /// <summary>
        /// 检查电池电量
        /// </summary>
        private bool CheckBattery()
        {
            if (MRStatus.Battery < 55) //电池规则检查
            {
                Console.WriteLine($"[CheckBattery]{MRStatus.MRID} Battery:{MRStatus.Battery}");
                //命令MR去充电
                if (!Docking && MRStatus.IOperatorStatus != IOperatorStatus.Docked)
                {
                    Console.WriteLine("命令MR去充电");
                    this.BeforeDockBattery = MRStatus.Battery;
                    this.Docking = WS.SendDockMission(MRStatus.MRID);
                    Console.WriteLine($"[DockingBattery]{MRStatus.MRID} Dock:{this.Docking}");
                }

                return false;
            }
            //todo:有充足电量，但是充电前电量是否达标
            this.Docking = false;
            return true;
        }



        #endregion

        public void RequestUpdateStatusAsync()
        {
            Console.WriteLine($"[{MRStatus.MRID} Request RequestUpdateStatusAsync]");
            OnMrRequestStatusRefresh?.Invoke(this, new MrIdArg
            {
                MRID = MRStatus.MRID
            });
        }
        public void RequestUpdateStatusSync()
        {
            OnMRStatusChange(WS.GetMRStatus(MRStatus.MRID));
        }
        public void SetWorkingStatus(bool working)
        {
            this.Working = working;
            MRStatus.IOperatorStatus = working ? IOperatorStatus.Busy : IOperatorStatus.Idle;
            MRStatus.MissionStatus = working ? MissionStatus.OnMission : MissionStatus.Standby;
        }
        /// <summary>
        /// 当更新最新的MRStatus事件
        /// </summary>
        /// <param name="eMrStatus"></param>
        public void OnMRStatusChange(MRStatus eMrStatus)
        {
            if (eMrStatus != null)
                this.MRStatus = eMrStatus;
        }
        /// <summary>
        /// 是否处于空闲状态
        /// </summary>
        /// <returns></returns>
        public bool IsIdle()
        {
            return MRStatus.IOperatorStatus == IOperatorStatus.Idle && MRStatus.MissionStatus == MissionStatus.Standby;
        }
    }
}
