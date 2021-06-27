using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using AgvcWorkFactory.Interfaces;
using Messages.Transfers;
using Messages.Transfers.Core;
using Protocol;
using Protocol.Mission;
using Protocol.Report;
using Protocol.Request;
using Utility.Helpers;

namespace AgvcWorkFactory.Tasks
{
    /// <summary>
    ///  机器人任务抽象基类
    /// </summary>
    public abstract class AbstractRobotTask : IRobotTask
    {
        #region IOC

        private IAgvReporter AgvReporter { get; }
        public IAgvRequester AgvRequester { get; }
        private IWS Ws { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        protected AbstractRobotTask(IAgvReporter agvReporter,IAgvRequester agvRequester, IWS ws)
        {
            AgvReporter = agvReporter;
            AgvRequester = agvRequester;
            Ws = ws;
        }

        #endregion

        /// <summary>
        ///     Transfer Request [TX501I]
        /// </summary>
        public List<IMessage> TrxRequestMessages { get; set; } = new();

        /// <summary>
        ///     当前任务执行的机器人对象
        /// </summary>
        private VirtualRobot VirtualRobot { get; set; }

        /// <summary>
        ///     任务ID
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString("N");

        /// <summary>
        ///     MRID
        /// </summary>
        public string MRID { get; set; }

        /// <summary>
        ///     设置MES->AGVC TX501i消息
        /// </summary>
        /// <param name="message"></param>
        public void AddTrxMessage(IMessage message)
        {
            if (message is not Tx501i) return;
            TrxRequestMessages.Add(message);
            OnTrxMessageAdded(message);
        }

        /// <summary>
        ///     Froms
        /// </summary>
        public List<TaskGoal> Froms { get; set; } = new();

        /// <summary>
        ///     Tos
        /// </summary>
        public List<TaskGoal> Tos { get; set; } = new();

        /// <summary>
        ///     开始执行
        /// </summary>
        /// <param name="virtualRobot"></param>
        public void Run(VirtualRobot virtualRobot)
        {
            MRID = virtualRobot.MRStatus.MRID;
            VirtualRobot = virtualRobot;

            ExecuteFromToRules();
        }

        /// <summary>
        ///     路径类型
        /// </summary>
        public TaskPathType PathType { get; set; }

        /// <summary>
        ///     任务类型
        /// </summary>
        public abstract RobotTaskType TaskType { get;  }

        /// <summary>
        ///     当MES TX501 请求消息被设置时.需要具体任务实体自行处理,
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnTrxMessageAdded(IMessage message)
        {
        }

        /// <summary>
        ///     實際任務单次執行步驟(From)
        /// </summary>
        protected abstract void OnRunFromTask(TaskGoal goal, int index);

        /// <summary>
        ///     實際任務单次執行步驟(To)
        /// </summary>
        protected abstract void OnRunToTask(TaskGoal goal, int index);

        /// <summary>
        ///     默认执行From To的规则,具体任务实例可以进行重写
        /// </summary>
        public virtual void ExecuteFromToRules()
        {
            //from
            var index = 0;
            foreach (var fromGoal in Froms)
            {
                index++;
                OnRunFromTask(fromGoal, index);
            }

            //to
            index = 0;
            foreach (var toGoal in Tos)
            {
                index++;
                OnRunToTask(toGoal, index);
            }
        }

        /// <summary>
        ///     等待IM報告
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recvFunc"></param>
        protected void WaitReport<T>(Func<T, bool> recvFunc) where T : BaseReport
        {
            var waitHandle = new AutoResetEvent(false);
            var reportTask = new AgvReport(MRID, Id, typeof(T), waitHandle)
            {
                AgreeCall = baseReport => { return recvFunc?.Invoke(baseReport as T) ?? true; }
            };
            //默认Report Timeout 配置覆盖最大超时设定【已废弃】
            // if (OptionalReportTimeouts.TryGetValue(typeof(T).Name, out var ms))
            // {
            //     if (ms > 0) timeout = ms;
            // }
            var agvError = false;
            var reportKey = reportTask.GetKey();
            Thread.Sleep(10);
            if (AgvReporter.TryAddWatch(reportTask)) //IM Report是不受控制的。有可能这里还没有AddWatch,通知却已经到达了。
                while (reportTask.Report == null && !agvError)
                {
                    VirtualRobot.State = $"等待AGV信号：{reportKey}中";
                    waitHandle.WaitOne(AgvReporter.GetAgvInitializeInterval()); //等待60s[可配置]
                    //======================================================================
                    //_waitHandle.WaitOne();
                    //挂起綫程等待，直到有任务上报或者轮询AGV为故障状态。
                    //=======================================================================
                    //在等待上报期间：如果AGV发生了故障或者通讯异常，那么需要每隔一分钟去轮询AGV状态，
                    //如果轮询到AGV IO 狀態=Initialize 时：此任务已无效。
                    //注意：机器人初始化之后没有接收到任务就标志为initial状态
                    //=======================================================================
                    if (reportTask.Report != null) //收到信号
                        break;
                    //检查agv状态
                    VirtualRobot.RequestUpdateStatusSync();
                    if (VirtualRobot.IsInitialize()) //agv状态有误
                        agvError = true; //抛弃本次任务
                }

            //已经成功Report
            var report = AgvReporter.GetReport(reportKey);
            if (report != null)
            {
                if (report is MissionDone done && !string.IsNullOrEmpty(done.Error)) //正对MissionDone特殊判断
                    throw new Exception($"MissionFail:{done.Error}");
                Console.WriteLine($"<<OK>><<{reportKey}>> 实际耗时：{reportTask.Ms} ms");
                AgvReporter.RemoveWatch(reportKey);
            }
            else
            {
                if (agvError)
                    throw new Exception($"<<ERR>><<{reportKey}>> 失败：AGV设备重启");
                throw new Exception($"<<ERR>><<{reportKey}>> 失败：获取Report失败");
            }
        }
        /// <summary>
        ///     等待IM请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recvFunc"></param>
        protected void WaitRequest<T>(Func<T, bool> recvFunc) where T : BaseRequest
        {
            var waitHandle = new AutoResetEvent(false);
            var requestTask = new AgvRequest(MRID, Id, typeof(T), waitHandle)
            {
                AgreeCall = baseRequest => { return recvFunc?.Invoke(baseRequest as T) ?? true; }
            };
          
            var agvError = false;
            var requestKey = requestTask.GetKey();
            Thread.Sleep(10);
            if (AgvRequester.TryAddWatch(requestTask)) //IM Request是不受控制的。有可能这里还没有AddWatch,通知却已经到达了。
                while (requestTask.Request == null && !agvError)
                {
                    VirtualRobot.State = $"等待AGV信号：{requestKey}中";
                    waitHandle.WaitOne(AgvReporter.GetAgvInitializeInterval()); //等待60s[可配置]
                    //======================================================================
                    //_waitHandle.WaitOne();
                    //挂起綫程等待，直到有任务上报或者轮询AGV为故障状态。
                    //=======================================================================
                    //在等待上报期间：如果AGV发生了故障或者通讯异常，那么需要每隔一分钟去轮询AGV状态，
                    //如果轮询到AGV IO 狀態=Initialize 时：此任务已无效。
                    //注意：机器人初始化之后没有接收到任务就标志为initial状态
                    //=======================================================================
                    if (requestTask.Request != null) //收到信号
                        break;
                    //检查agv状态
                    VirtualRobot.RequestUpdateStatusSync();
                    if (VirtualRobot.IsInitialize()) //agv状态有误
                        agvError = true; //抛弃本次任务
                }

            //已经成功Request
            var request = AgvRequester.GetRequest(requestKey);
            if (request != null)
            {
          
                Console.WriteLine($"<<OK>><<{requestKey}>> 实际耗时：{requestTask.Ms} ms");
                AgvRequester.RemoveWatch(requestKey);
            }
            else
            {
                if (agvError)
                    throw new Exception($"<<ERR>><<{requestKey}>> 失败：AGV设备重启");
                throw new Exception($"<<ERR>><<{requestKey}>> 失败：获取Request失败");
            }
        }
        /// <summary>
        ///     發送Mission
        /// </summary>
        /// <param name="mission"></param>
        protected BaseMission.Response SendMission(BaseMission mission)
        {
            mission.MRID = MRID;
            mission.MissionID = Id;
             
            var response =   AsyncHelper.RunSync(() => { return Ws.DispatchAsync<BaseMission.Response>(mission); });
            if (response==null)
            {
                throw new Exception($"无法对AGV执行{nameof(BaseMission)}操作。请检查网络通讯是否正常。");
            }

            return response;
        }

        /// <summary>
        ///     AGVC->MES
        /// </summary>
        /// <param name="message"></param>
        protected void AGVC2MES(IMessage message)
        {
            //todo:由于尚未对接MES 的IBM MQ,
            //暂时不写
        }

        #region OptionalReportTimeouts

        /// <summary>
        ///     用于超时时间配置
        /// </summary>
        [Obsolete("已废弃")] private static Dictionary<string, double> _rpts;

        [Obsolete("已废弃")]
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
                        typeof(TransportEnd)
                    };
                    foreach (var reportType in reportTypes)
                        _rpts.Add(reportType.Name,
                            reportType.GetCustomAttribute<TimeoutAttribute>()?.Milliseconds ?? 0);
                }

                return _rpts;
            }
        }

        #endregion

        #region 执行标准指令组

        /// <summary>
        ///     执行标准Pick操作
        /// </summary>
        /// <param name="pick"></param>
        protected void RunPickMission(Pick pick, Action<Base> action = null)
        {
            //1.MrStatus 任务执行前已经获取了状态,这里不需要重新获取
            //VirtualRobot.RequestUpdateStatusAsync(); 
            //2.Pick
            SendMission(pick);
            //3.Arrived
            WaitReport<Arrived>(arrived =>
            {
                action?.Invoke(arrived);
                return true; //ack
            });
            //3.1 Transport Confirm
            WaitRequest<TransportConfirm>(transportConfirm=> {
                action?.Invoke(transportConfirm);
                return true;//ack
            });
            //4.Transfer End
            WaitReport<TransportEnd>(transportEnd =>
            {
                action?.Invoke(transportEnd);
                return true; //ack
            });
            //5.Pick MissionDone
            WaitReport<MissionDone>(missionDone =>
            {
                action?.Invoke(missionDone);
                return true; //ack
            });
        }

        /// <summary>
        ///     执行标准Drop操作
        /// </summary>
        /// <param name="drop"></param>
        /// <param name="action"></param>
        protected void RunDropMission(Drop drop, Action<Base> action = null)
        {
            //1.MrStatus
            //VirtualRobot.RequestUpdateStatusAsync(); 
            //2.Drop
            SendMission(drop);
            //3.Arrived
            WaitReport<Arrived>(arrived =>
            {
                action?.Invoke(arrived);
                return true; //ack
            });
            //3.1 Transport Confirm
            WaitRequest<TransportConfirm>(transportConfirm => {
                action?.Invoke(transportConfirm);
                return true;//ack
            });
            //4.Transfer End
            WaitReport<TransportEnd>(transportEnd =>
            {
                action?.Invoke(transportEnd);
                return true; //ack
            });
            //5.Pick MissionDone
            WaitReport<MissionDone>(missionDone =>
            {
                action?.Invoke(missionDone);
                return true; //ack
            });
        }

        #endregion
    }
}