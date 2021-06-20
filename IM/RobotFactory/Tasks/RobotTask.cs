using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AgvcUtility;
using AgvUtility;
using Messages.Transfers.Core;
using Microsoft.VisualBasic;
using Protocol;
using Protocol.Mission;
using RobotDefine;

namespace RobotFactory
{
    public enum RobotTaskType
    {
        /// <summary>
        /// Stock to EQP
        /// 执行搬运单条指令
        /// </summary>
        Stock2EQP
    }

    public class TaskTypeAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Attribute" /> class.</summary>
        public TaskTypeAttribute(RobotTaskType taskType)
        {
            TaskType = taskType;
        }

        public RobotTaskType TaskType { get; }
    }

    public interface IRobotTask
    {
        /// <summary>
        ///  任务ID
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Transfer Request [TX501I]
        /// </summary>
        IMessage TransferRequestMessage { get; set; }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="virtualRobot"></param>
        void Run(VirtualRobot virtualRobot);
    }
    public abstract class RobotTask : IRobotTask
    {
        private AutoResetEvent _waitHandle;

        /// <summary>
        ///  任务ID
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// MRID
        /// </summary>
        public string MRID => VirtualRobot.MRStatus.MRID;

        /// <summary>
        /// Transfer Request [TX501I]
        /// </summary>
        public IMessage TransferRequestMessage { get; set; }
        /// <summary>
        /// 實際任務執行步驟
        /// </summary>
        protected abstract void OnRun();

        public void Run(VirtualRobot virtualRobot)
        {
            this.VirtualRobot = virtualRobot;
            this.OnRun();

        }

        protected VirtualRobot VirtualRobot { get; set; }
        /// <summary>
        /// 等待IM報告
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recvFunc"></param>
        /// <param name="timeout">超時時間(MS)</param>
        protected void WaitReport<T>(Func<T, bool> recvFunc, int timeout = 5000) where T : BaseReport
        {
            _waitHandle = new AutoResetEvent(false);
            var reportTask = new ReportHanlder
            {
                MissionId = Id,
                Type = typeof(T),
                AutoResetEvent = _waitHandle,
            };
            IMReporter.Watch(reportTask);
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkMethod), _waitHandle);
            if (_waitHandle.WaitOne(timeout, false))
            {
                Console.WriteLine("Work method signaled.");

            }
            else
            {
                Console.WriteLine("Timed out waiting for work " + "method to signal.");

            }
            // AsyncHelper.RunSync(() =>
            //     {
            //         return Task.Run(() =>
            //         {
            //             _waitHandle.WaitOne(); //挂起綫程等待
            //             var received = recvFunc(reportTask.Report as T);
            //             reportTask.Received = received;
            //
            //         }).TimeoutAfter(TimeSpan.FromMilliseconds(timeout)); //超時退出
            //
            //     });

            IMReporter.Remove(typeof(T),Id);

        }

        private void WorkMethod(object stateInfo)
        {
            Console.WriteLine("Work starting.");

            // Simulate time spent working.

            Thread.Sleep(new Random().Next(100, 2000));

            // Signal that work is finished.

            Console.WriteLine("Work ending.");

            ((AutoResetEvent)stateInfo).Set();
        }

        /// <summary>
        /// 發送Mission
        /// </summary>
        /// <param name="mission"></param>
        protected Protocol.BaseMission.Response SendMission(BaseMission mission)
        {
            mission.MRID = MRID;
            mission.MissionID = Id;
            return WS.Dispatch<Protocol.BaseMission.Response>(mission);
        }

    }
}