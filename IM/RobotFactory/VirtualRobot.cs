using System;
using System.Collections.Generic;
using System.Threading;
using AgvUtility;
using RobotDefine;

namespace RobotFactory
{
    /// <summary>
    /// 虚拟机器人
    /// </summary>
    public class VirtualRobot:IDisposable
    {
        /// <summary>
        /// 机器人实时状态
        /// </summary>
        public MRStatus MRStatus { get; set; }
        /// <summary>
        /// 当前電池電量百分比（充电前）,充电停止后会自动更新此电量
        /// </summary>
        public double Battery { get; set; }

        #region 任务

        /// <summary>
        /// 当前待处理任务
        /// </summary>
        private readonly Queue<IRobotTask> Tasks = new Queue<IRobotTask>();
        private readonly object _locker = new();
        private CancellationTokenSource _cancelTokenSource;
        private Thread _watchThread;
        public int TaskCount => Tasks.Count;

        public void AddTask(IRobotTask task)
        {
            Tasks.Enqueue(task);
            if (Tasks.Count>0&&_watchThread==null)
            {
                this.Start();
            }
        }
        /// <summary>执行与释放或重置非托管资源相关的应用程序定义的任务。</summary>
        public void Dispose()
        {
            this.Stop();
        }
        public void Start()
        {
            _cancelTokenSource = new CancellationTokenSource();
            _watchThread = new Thread(QueueWatchThread) { IsBackground = true };
            _watchThread.Start();
        }

        private void QueueWatchThread()
        {
            while (!_cancelTokenSource.IsCancellationRequested)
            {
                //Console.WriteLine("队列内任务量:" + (TaskQueue.Count));//输出时间 毫秒
                lock (_locker)
                {
                    while (Tasks.Count > 0)
                    {
                        //todo:檢查MR狀態是否可以執行任務

                        var task = Tasks.Dequeue();//任務是否可以出列
                        task.Run(this);//運行任務
                    }
                }
            }
        }

        public void Stop()
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
        #endregion

        public void UpdateStatus()
        {
            var response = AsyncHelper.RunSync(() => WS.DispatchAsync<Protocol.Query.MRStatus.Response>(new Protocol.Query.MRStatus
            {
                MRID = this.MRStatus.MRID
            }));
            this.MRStatus = response.MRStatus;
        }
    }
}
