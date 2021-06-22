using System;
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
        public string MrId { get; set; }
        public string MissionId { get; set; }
        public Type Type { get; set; }
        public AutoResetEvent AutoResetEvent { get; set; }
        public BaseReport Report { get; set; }
        public bool Received { get; set; }
        public DateTime CreateTime { get; set; }
        public double Ms { get; set; }
    }

    public static class AgvReporter
    {
        private static readonly List<AgvReport> Reports = new List<AgvReport>();
        private static object _locker = new object();
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        static AgvReporter()
        {
            AgvStatusWatcher = new RobotStatusWatcher();
            AgvStatusWatcher.MrStatusReceived += AgvStatusWatcher_MrStatusReceived;
        }

        private static void AgvStatusWatcher_MrStatusReceived(object sender, MrStatusEventArg e)
        {

            if (e.MrStatus.IOperatorStatus == IOperatorStatus.Initialize ||
                e.MrStatus.MissionStatus == MissionStatus.Initialize)
            {
                OnAgvcInitialize(e.MrStatus.MRID);
            }
            else
            {
                lock (_locker)
                {
                    if (Reports.Any(p => p.MrId == e.MrStatus.MRID)) //还在监控列表中
                    {
                        AgvcTimer.SetTimeout(60 * 1000, delegate
                        {
                            AgvStatusWatcher.Watch(e.MrStatus.MRID); //1分钟后重新加入监控队列
                        });
                    }
                }

            }

        }

        /// <summary>
        /// AGVC在汇报期间重启
        /// </summary>
        /// <param name="mrid"></param>
        private static void OnAgvcInitialize(string mrid)
        {
            lock (_locker)
            {
                var mrReports = Reports.Where(p => p.MrId == mrid);
                foreach (var agvReport in mrReports)
                {
                    agvReport.Report = null;
                    agvReport.Ms = DateTime.Now.Subtract(agvReport.CreateTime).TotalMilliseconds;
                    agvReport.AutoResetEvent.Set(); // 发送信号
                }

                Reports.RemoveAll(p => p.MrId == mrid);//clear all mrid reports
            }
        }

        private static RobotStatusWatcher AgvStatusWatcher { get; }
        public static void Watch(AgvReport agvReport)
        {
            lock (_locker)
            {
                agvReport.CreateTime = DateTime.Now;
                Reports.Add(agvReport);
                AgvStatusWatcher.Watch(agvReport.MrId);//监控Agv状态
            }
        }

        public static void Remove(Type type, string missionId)
        {
            lock (_locker)
            {
                Reports.RemoveAll(p => p.Type == type && p.MissionId == missionId);
            }
        }
        /// <summary>
        /// IMG->AGVC 汇报了状态
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public static Response OnReport(BaseReport report)
        {
            lock (_locker)
            {
                var needReport = Reports.FirstOrDefault(p => p.Type == report.Type && p.MissionId == report.MissionID);
                var received = true; //默认agree = true
                if (needReport != null)
                {
                    needReport.Report = report;
                    needReport.Ms = DateTime.Now.Subtract(needReport.CreateTime).TotalMilliseconds;
                    needReport.AutoResetEvent.Set(); // 发送信号
                    received = needReport.Received;
                }

                Remove(report.Type, report.MissionID);
                return report.GetResponse(received);
            }

        }
    }
}