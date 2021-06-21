using System;
using System.Threading;
using System.Threading.Tasks;
using AgvUtility;
using Messages.Transfers.Core;
using Protocol;
namespace RobotFactory.Tasks
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
            virtualRobot.OnMission(true);
            this.OnRun();

            virtualRobot.OnMission(false);

        }

        protected VirtualRobot VirtualRobot { get; set; }
        /// <summary>
        /// 等待IM報告
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recvFunc"></param>
        /// <param name="timeout">超時時間(MS)</param>
        protected void WaitReport<T>(Func<T, bool> recvFunc, int timeout = 50000) where T : BaseReport
        {
            var _waitHandle = new AutoResetEvent(false);
            var reportTask = new ReportHanlder
            {
                MissionId = Id,
                Type = typeof(T),
                AutoResetEvent = _waitHandle,
            };
            AsyncHelper.RunSync(() =>
            {
                return Task.Run(() =>
                {
                    IMReporter.Watch(reportTask);
                    Console.WriteLine($"[期待IM汇报]{typeof(T).Name} 最大超时设定:{timeout}ms");
                    _waitHandle.WaitOne(TimeSpan.FromMilliseconds(timeout)); //挂起綫程等待
                    
                    if (reportTask.Report!=null)
                    {  
                        Console.WriteLine($"[收到IM汇报]{typeof(T).Name}  耗时：{reportTask.Ms} ms");
                        var received = recvFunc(reportTask.Report as T);
                        reportTask.Received = received;
                    }
                    else
                    {
                        Console.WriteLine($"[IM汇报已超时]{typeof(T).FullName}");
                    }
                    

                });
            });
            
            IMReporter.Remove(typeof(T), Id);

        }


        /// <summary>
        /// 發送Mission
        /// </summary>
        /// <param name="mission"></param>
        protected Protocol.BaseMission.Response SendMission(BaseMission mission)
        {
            mission.MRID = MRID;
            mission.MissionID = Id;

            return AsyncHelper.RunSync(() =>
            {
               return WS.DispatchAsync<Protocol.BaseMission.Response>(mission);
            });
        }

    }
}