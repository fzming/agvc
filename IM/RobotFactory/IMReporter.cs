using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Protocol;
using Protocol.Report;

namespace RobotFactory
{
    public class ReportHanlder
    {
        public string MissionId { get; set; }
        public Type Type { get; set; }
        public AutoResetEvent AutoResetEvent { get; set; }
        public BaseReport Report { get; set; }
        public bool Received { get; set; }
        public DateTime CreateTime { get; set; }
        public double Ms { get; set; }
    }

    public static class IMReporter
    {
        private static List<ReportHanlder> reportHanlders = new List<ReportHanlder>();
        public static void Watch(ReportHanlder reportHanlder)
        {
            reportHanlder.CreateTime = DateTime.Now;
            
            reportHanlders.Add(reportHanlder);
        }

        public static void Remove(Type type, string missionId)
        {
            reportHanlders.RemoveAll(p => p.Type == type && p.MissionId == missionId);
        }

        public static Response OnReport(BaseReport report)
        {
            var handle = reportHanlders.FirstOrDefault(p => p.Type == report.Type && p.MissionId == report.MissionID);
            var received = true;
            if (handle != null)
            {
                handle.Report = report;
                handle.Ms = DateTime.Now.Subtract(handle.CreateTime).TotalMilliseconds;
                handle.AutoResetEvent.Set();// 发送信号
                received = handle.Received;
            }
            
            Remove(report.Type, report.MissionID);
            return report.GetResponse(received);


        }
    }
}