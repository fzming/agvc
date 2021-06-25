using System;
using System.Collections.Generic;
using System.Threading;
using AgvcWorkFactory.Tasks;
using RobotDefine;

namespace AgvcWorkFactory
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
        /// 充电前電量百分
        /// </summary>
        public double BeforeDockBattery { get; set; }
        /// <summary>
        /// 正处于充电状态
        /// </summary>
        public bool Docking { get; set; }
        /// <summary>
        /// 正处于任务执行状态
        /// </summary>
        public bool Working { get; set; }
        #region 任务
        /// <summary>
        /// 当前状态描述
        /// </summary>
         public string State { get; set; }
        /// <summary>
        /// 当前待处理任务
        /// </summary>
        private readonly Queue<IRobotTask> _tasks = new Queue<IRobotTask>();
        private CancellationTokenSource _cancelTokenSource;
        private Thread _watchThread;
        public int TaskCount => _tasks.Count;
        private readonly object _syncRoot = new object();
        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        
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
            //未指定MR，需要等待加入到队列
            if (string.IsNullOrEmpty(task.MRID))
            {
                lock (_syncRoot)
                {
                    //锁住，当MR正在做任务时，不允许新的任务加入。
                    AddTaskInQueque(task);
                }
            }
            else
            {
                //已指定MR，可直接加入队列
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
                _waitHandle.Close();
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
                lock (_syncRoot) //锁住防止新的Task被加入
                {
                    if (!Working && _tasks.Count > 0) //只有在非工作状态标记下才执行
                    {
                        if (!IsRobotReadyWork())
                        {
                            //不适合做任务的原因================
                            //1.电量不足
                            //2.不处于空闲状态
                            //3.当前任务因素
                            //================================
                            RequestUpdateStatusAsync();
                           // Console.WriteLine($"[{MRStatus.MRID}]开启状态轮询");

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
                                SetWorkingStatus(false);
                                
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                throw;
                            }
                            finally
                            {
                                //任务执行完毕后，需要更新MR状态
                                RequestUpdateStatusSync();
                                SetWorkingStatus(false);
                            }

                        }
                    }
                }

                if ((task != null || rollUpdateStatus) && _tasks.Count > 0)
                {
                    //Thread.Sleep(300);
                    Thread.Sleep(rollUpdateStatus ? 5000 : 500); //轮询为5秒一次
                }
                else
                {
                    Console.WriteLine($"[{MRStatus.MRID}] 工作线程已休眠，等待任务。");
                    SetWorkingStatus(false);
                    _waitHandle.WaitOne();
                }

            }
        }
        /// <summary>
        /// 檢查MR狀態是否可以執行任務
        /// </summary>
        /// <returns></returns>
        public bool IsRobotReadyWork()
        {
            if (MRStatus.IOperatorStatus == IOperatorStatus.Docked) //当前正在充电
            {
                return CheckBattery();//仅检查充电是否可以完毕
            }
            return !Working&&CheckBattery() && (IsIdle()|| IsInitialize());
        }
        /// <summary>
        /// 检查电池电量
        /// 当电池电量低时将自动命令MR去充电,同时根据充电前的电量百分比,决定了MR何时苏醒再次接收工作,
        /// 注意:机器人处于充电状态下将无法接收任务引擎的随机任务指派,除非在安排任务时指示本MR执行,任务会加入到MR的待处理队列.等就绪时:优先执行.
        /// </summary>
        private bool CheckBattery()
        {
            if (MRStatus.Battery < 30) //电量不足[可配置]
            {
                Console.WriteLine($"[CheckBattery]{MRStatus.MRID} Battery:{MRStatus.Battery}");
                //命令MR去充电
                if (!Docking && MRStatus.IOperatorStatus != IOperatorStatus.Docked)
                {
                    Console.WriteLine("命令MR去充电");
                    this.BeforeDockBattery = MRStatus.Battery;
                    this.Docking = WS.SendDockMission(MRStatus.MRID);
                    Console.WriteLine($"[DockingBattery]{MRStatus.MRID} Dock:{this.Docking}");
                    this.State = "充电中";
                }

                return false;
            }
            if (Docking) 
            { 
                //当前正在充电中,且30%(充电前)<电量<70%,若充电前电量小于30%.不允许继续分派任务[可配置]
                if (this.BeforeDockBattery < 30 && MRStatus.Battery < 70)
                {
                    Console.WriteLine($"充电中（保养策略）充电前：{this.BeforeDockBattery} 当前电量：{MRStatus.Battery}");
                    this.State = "充电中（保养策略）";
                    return false;
                }
                //满足电量要求：可以指派任务
                this.Docking = false;
            }
            return true;
        }


        #endregion
        /// <summary>
        /// 请求异步更新MR状态
        /// </summary>
        public void RequestUpdateStatusAsync()
        {
            Console.WriteLine($"[{MRStatus.MRID} Request RequestUpdateStatusAsync]");
            OnMrRequestStatusRefresh?.Invoke(this, new MrIdArg
            {
                MRID = MRStatus.MRID
            });
        }
        /// <summary>
        /// 请求同步更新MR状态
        /// </summary>
        public void RequestUpdateStatusSync()
        {
            OnMRStatusChange(WS.GetMRStatus(MRStatus.MRID));
        }
        /// <summary>
        /// 设置MR状态
        /// </summary>
        /// <param name="working"></param>
        public void SetWorkingStatus(bool working)
        {
            this.Working = working;
            this.State = "空闲";
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
            {
                Console.WriteLine($"{MRStatus.MRID} StatusChange,{MRStatus.IOperatorStatus.ToString()},{MRStatus.MissionStatus}");
                this.MRStatus = eMrStatus;
            }
        }
        /// <summary>
        /// 是否处于空闲状态
        /// </summary>
        /// <returns></returns>
        public bool IsIdle()
        {
            return MRStatus.IOperatorStatus == IOperatorStatus.Idle && MRStatus.MissionStatus == MissionStatus.Standby;
        }
        /// <summary>
        /// 是否处于初始化状态.MR重启后,一直会处于此状态.
        /// </summary>
        /// <returns></returns>
        public bool IsInitialize()
        {
            return (MRStatus.IOperatorStatus == IOperatorStatus.Initialize &&
                    MRStatus.MissionStatus == MissionStatus.Initialize);
        }
    }
}
