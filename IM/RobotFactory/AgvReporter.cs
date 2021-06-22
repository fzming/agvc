using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AgvcUtility;
using Protocol;
using Protocol.Report;

using RobotDefine;

namespace RobotFactory
{
    public class AgvReport
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AgvReport(string mrId, string missionId, Type type, AutoResetEvent autoResetEvent)
        {
            MrId = mrId;
            MissionId = missionId;
            Type = type;
            AutoResetEvent = autoResetEvent;
        }

        public string MrId { get; set; }
        public string MissionId { get; set; }
        public Type Type { get; set; }
        public AutoResetEvent AutoResetEvent { get; set; }
        public BaseReport Report { get; set; }
        public bool Received { get; set; }
        public DateTime CreateTime { get; set; }
        public double Ms { get; set; }
        public string WatchId { get; set; }
    }
    /// <summary>
    /// AGV设备回调监控
    /// </summary>
    public class AgvReporter
    {
        private static readonly Lazy<AgvReporter> Instancelock = new Lazy<AgvReporter>(
            () => new AgvReporter());

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AgvReporter()
        {
            AgvStatusWatcher = new RobotStatusWatcher();
            AgvStatusWatcher.MrStatusReceived += AgvStatusWatcher_MrStatusReceived;
            AgvStatusWatcher.MrStatusError += AgvStatusWatcher_MrStatusError;
        }

        public static AgvReporter Instance => Instancelock.Value;
        private static object _locker = new object();
        private readonly ConcurrentDictionary<string, AgvReport> Reports =
                    new ConcurrentDictionary<string, AgvReport>();


        private void AgvStatusWatcher_MrStatusError(object sender, MrStatusErrorArg e)
        {
            if (Reports.TryGetValue(e.MRID, out var agvReport)) //MR还在监控状态列表中
            {
                RetryWatch(e.MRID, agvReport.WatchId, "故障：" + e.Error);
            }
        }

        private void AgvStatusWatcher_MrStatusReceived(object sender, MrStatusEventArg e)
        {
            if (Reports.TryGetValue(e.MrStatus.MRID, out var agvReport)) //MR还在监控状态列表中
            {
                Console.WriteLine($"{e.MrStatus.MRID}: IOperatorStatus={e.MrStatus.IOperatorStatus} MissionStatus={e.MrStatus.MissionStatus}");
                //初始化MR状态
                if (e.MrStatus.IOperatorStatus == IOperatorStatus.Initialize &&
                    e.MrStatus.MissionStatus == MissionStatus.Initialize)
                {
                    OnAgvError(e.MrStatus.MRID);
                }
                else
                {

                    RetryWatch(e.MrStatus.MRID, agvReport.WatchId, "監控循環時間已到");

                }
            }

        }

        private void RetryWatch(string mrStatusMrid, string watchId, string reason, int interval = 60 * 1000)
        {
            AgvcTimer.SetTimeout(interval, () =>
            {
                lock (_locker)
                {
                    if (Reports.TryGetValue(mrStatusMrid, out var agvReport)) //还在监控列表中
                    {
                        if (agvReport.WatchId != watchId) return;

                        Console.WriteLine(
                            $"{mrStatusMrid}:重新加入监控队列[{agvReport.Type.Name}] [{DateTime.Now:T}]，原因：{reason}");
                        AgvStatusWatcher.Watch(mrStatusMrid); //重新加入监控队列
                    }
                }
            });

        }

        /// <summary>
        /// AGV在汇报期间重启
        /// </summary>
        /// <param name="mrid"></param>
        private void OnAgvError(string mrid)
        {

            if (Reports.TryGetValue(mrid, out var agvReport))
            {
                agvReport.Report = null;
                agvReport.Ms = DateTime.Now.Subtract(agvReport.CreateTime).TotalMilliseconds;
                RemoveWatch(mrid);
                agvReport.AutoResetEvent.Set(); // 发送信号

            }
        }

        private RobotStatusWatcher AgvStatusWatcher { get; }

        public void Watch(AgvReport agvReport)
        {
            lock (_locker)
            {
                agvReport.CreateTime = DateTime.Now;
                agvReport.WatchId = Guid.NewGuid().ToString("N");
                if (Reports.TryAdd(agvReport.MrId, agvReport))
                {

                    Console.WriteLine($"[加入监控] {agvReport.MrId} {agvReport.Type.Name}");
                    AgvStatusWatcher.Watch(agvReport.MrId); //监控Agv状态
                }
                else
                {
                    Console.Error.WriteLine($"{agvReport.MrId} 不能重复加入监控");
                }
            }
        }

        private void RemoveWatch(string mrid)
        {
            lock (_locker)
            {
                if (Reports.TryRemove(mrid, out var agvReport))
                {
                    Console.WriteLine($"[移除监控] {agvReport.MrId} {agvReport.Type.Name}");
                }
            }
        }

        /// <summary>
        /// IMG->AGVC 汇报了状态
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public Response OnReport(BaseReport report)
        {
            Console.WriteLine($"[IM -> REPORT] {report.MRID} ->{report.Type.Name}");
            var received = true; //默认agree = true
            if (Reports.TryGetValue(report.MRID, out var agvReport))
            {
                if (agvReport.Type == report.Type && agvReport.MissionId == report.MissionID) //有可能回报的类型跟现在监控的类型不一样。此时应该抛弃。
                {
                    agvReport.Report = report;
                    agvReport.Ms = DateTime.Now.Subtract(agvReport.CreateTime).TotalMilliseconds;
                    RemoveWatch(report.MRID);
                    agvReport.AutoResetEvent.Set(); // 发送信号
                    received = agvReport.Received;
                }
                else
                {
                    Console.Error.WriteLine($"report error:{report.MRID}->{report.Type.Name} 状态不正确");
                }
            }
            else
            {
                Console.Error.WriteLine($"report error:{report.MRID}->{report.Type.Name} 未找到可释放句柄，请检查是否提前释放了此AGV的锁信号");
            }
            return report.GetResponse(received);
        }
    }
}