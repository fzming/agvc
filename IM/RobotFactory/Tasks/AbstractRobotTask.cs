using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using AgvUtility;

using Messages.Transfers;
using Messages.Transfers.Core;

using Protocol;
using Protocol.Mission;
using Protocol.Report;

namespace RobotFactory.Tasks
{
    public enum RobotTaskType
    {
        /// <summary>
        /// 执行搬运指令
        /// </summary>
        Transfer
    }

    public class TaskGoal
    {
        /// <summary>
        /// 貨箱號碼
        /// </summary>
        public string BoxID { get; set; }
        /// <summary>
        /// 目標點
        /// </summary>
        public string Goal { get; set; }
        /// <summary>
        /// 目標儲位
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 晶圓片數
        /// </summary>
        public int WaferCount { get; set; }
        /// <summary>
        /// 是否来至MES 的Transfer Request消息，如果是手工派发的任务，此字段将为NULL
        /// </summary>
        public IMessage TrxMessage { get; set; }
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
        /// MRID
        /// </summary>
        public string MRID { get; set; }
        /// <summary>
        /// 设置MES->AGVC TX501i消息
        /// </summary>
        /// <param name="message"></param>
        void AddTrxMessage(IMessage message);
        /// <summary>
        /// Froms
        /// </summary>
        List<TaskGoal> FromGoals { get; set; }
        /// <summary>
        /// Tos
        /// </summary>
        List<TaskGoal> ToGoals { get; set; }
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="virtualRobot"></param>
        void Run(VirtualRobot virtualRobot);
    }
    public abstract class AbstractRobotTask : IRobotTask
    {
        /// <summary>
        ///  任务ID
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// MRID
        /// </summary>
        public string MRID { get; set; }

        /// <summary>
        /// Transfer Request [TX501I]
        /// </summary>
        public List<IMessage> TrxRequestMessages { get; set; } = new List<IMessage>();

        /// <summary>
        /// 设置MES->AGVC TX501i消息
        /// </summary>
        /// <param name="message"></param>
        public void AddTrxMessage(IMessage message)
        {
            if (message is not Tx501i) return;
            this.TrxRequestMessages.Add(message);
            this.OnTrxMessageAdded(message);
        }

        #region OptionalReportTimeouts

        private static Dictionary<string, double> _rpts;
        private static Dictionary<string, double> OptionalReportTimeouts
        {
            get
            {
                if (_rpts == null)
                {
                    _rpts = new Dictionary<string, double>();
                    var reportTypes = new[]
                    {
                        typeof(Arrived),
                        typeof(MissionDone),
                        typeof(MissionFail),
                        typeof(TransportEnd),
                    };
                    foreach (var reportType in reportTypes)
                    {
                        _rpts.Add(reportType.Name,
                            reportType.GetCustomAttribute<TimeoutAttribute>()?.Milliseconds ?? 0);
                    }
                }

                return _rpts;
            }
        }

        #endregion
        protected virtual void OnTrxMessageAdded(IMessage message) { }

        /// <summary>
        /// Froms
        /// </summary>
        public List<TaskGoal> FromGoals { get; set; } = new List<TaskGoal>();

        /// <summary>
        /// Tos
        /// </summary>
        public List<TaskGoal> ToGoals { get; set; } = new List<TaskGoal>();
        /// <summary>
        /// 實際任務单次執行步驟(From)
        /// </summary>
        protected abstract void OnRunFromTask(TaskGoal goal, int index);
        /// <summary>
        /// 實際任務单次執行步驟(To)
        /// </summary>
        protected abstract void OnRunToTask(TaskGoal goal, int index);

        public void Run(VirtualRobot virtualRobot)
        {
            this.MRID = virtualRobot.MRStatus.MRID;
            this.VirtualRobot = virtualRobot;

            this.ExecuteFromToRules();

        }
        /// <summary>
        /// 默认执行From To的规则,具体任务实例可以进行重写
        /// </summary>
        public virtual void ExecuteFromToRules()
        {
            //from
            var index = 0;
            foreach (var fromGoal in this.FromGoals)
            {
                index++;
                this.OnRunFromTask(fromGoal, index);
            }
            //to
            index = 0;
            foreach (var toGoal in this.ToGoals)
            {
                index++;
                this.OnRunToTask(toGoal, index);
            }
        }

        protected VirtualRobot VirtualRobot { get; set; }
        /// <summary>
        /// 等待IM報告
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recvFunc"></param>
        protected void WaitReport<T>(Func<T, bool> recvFunc) where T : BaseReport
        {
            var _waitHandle = new AutoResetEvent(false);
            var reportTask = new AgvReport(MRID, Id, typeof(T), _waitHandle);
            //默认Report Timeout 配置覆盖最大超时设定【已废弃】
            // if (OptionalReportTimeouts.TryGetValue(typeof(T).Name, out var ms))
            // {
            //     if (ms > 0) timeout = ms;
            // }
            AsyncHelper.RunSync(() =>
            {
                return Task.Run(() =>
                {
                    var agvReporter = AgvReporter.Instance;
                    agvReporter.Watch(reportTask);
                    // Console.WriteLine($"[期待IM汇报]{typeof(T).Name} 挂起綫程等待，直到有任务上报或者轮询AGV为故障状态 ");
                    VirtualRobot.State = $"等待AGV信号：{typeof(T).Name}中";
                    _waitHandle.WaitOne();
                    //挂起綫程等待，直到有任务上报或者轮询AGV为故障状态。
                    //=======================================================================
                    //在等待上报期间：如果AGV发生了故障或者通讯异常，那么需要每隔一分钟去轮询AGV状态，
                    //如果轮询到AGV IO 狀態=Initialize 时：此任务已无效。
                    //注意：机器人初始化之后没有接收到任务就标志为initial状态
                    //=======================================================================
                    if (reportTask.Report != null)
                    {
                        Console.WriteLine($"{MRID}->[{typeof(T).Name}] 实际耗时：{reportTask.Ms} ms");
                        var received = recvFunc(reportTask.Report as T);
                        reportTask.Received = received;
                    }
                    else
                    {
                        Console.WriteLine($"{MRID}->[{typeof(T).Name}] Report ERROR");
                    }
                });
            });

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
        /// <summary>
        /// AGVC->MES
        /// </summary>
        /// <param name="message"></param>
        protected void AGVC2MES(IMessage message)
        {

        }

        #region 执行标准指令组
        /// <summary>
        /// 执行Pick操作
        /// </summary>
        /// <param name="pick"></param>
        protected void RunPickMission(Pick pick, Action<BaseReport> reportAction = null)
        {
            //1.MrStatus
            //VirtualRobot.RequestUpdateStatusAsync(); 
            //2.Pick
            SendMission(pick);
            //3.Arrived
            WaitReport<Arrived>(arrived =>
            {
                reportAction?.Invoke(arrived);
                return true;//ack
            });
            //4.Transfer End
            WaitReport<TransportEnd>(transportEnd =>
            {
                reportAction?.Invoke(transportEnd);
                return true;//ack
            });
            //5.Pick MissionDone
            WaitReport<MissionDone>(missionDone =>
            {
                reportAction?.Invoke(missionDone);
                return true;//ack
            });
        }

        /// <summary>
        /// 执行Drop操作
        /// </summary>
        /// <param name="drop"></param>
        /// <param name="reportAction"></param>
        protected void RunDropMission(Drop drop, Action<BaseReport> reportAction = null)
        {
            //1.MrStatus
            //VirtualRobot.RequestUpdateStatusAsync(); 
            //2.Drop
            SendMission(drop);
            //3.Arrived
            WaitReport<Arrived>(arrived =>
            {
                reportAction?.Invoke(arrived);
                return true;//ack
            });
            //4.Transfer End
            WaitReport<TransportEnd>(transportEnd =>
            {
                reportAction?.Invoke(transportEnd);
                return true;//ack
            });
            //5.Pick MissionDone
            WaitReport<MissionDone>(missionDone =>
            {
                reportAction?.Invoke(missionDone);
                return true;//ack
            });
        }

        #endregion
    }
}