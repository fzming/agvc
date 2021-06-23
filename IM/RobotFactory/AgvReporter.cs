using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Protocol;
using Protocol.Report;

using RobotDefine;
using RobotFactory.Interfaces;

namespace RobotFactory
{
    /// <summary>
    /// AGV设备回调监控
    /// </summary>
    public class AgvReporter : IAgvReporter
    {
        
        private readonly ConcurrentDictionary<string, AgvReport> Watchs =
            new ConcurrentDictionary<string, AgvReport>();

        public bool TryAddWatch(AgvReport agvReport)
        {
            var key = agvReport.GetKey();
            return Watchs.TryAdd(key, agvReport);
        }

        public void RemoveWatch(string key)
        {
            if (Watchs.TryRemove(key, out var agvReport))
            {
                agvReport.WaitHandle?.Close();
                Console.WriteLine($"[移除监控] {agvReport.MrId} {key}");
            }
        }

        /// <summary>
        /// IMG->AGVC 汇报了状态
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public Response OnReport(BaseReport report)
        {
            Console.WriteLine($"<<Report>> {report.MRID}->{report.Type.FullName}");
            var reportType = report.Type;

            #region 针对MissionFail特殊处理

            if (reportType==typeof(MissionFail)) //agv报告了MissionFail
            {
                reportType = typeof(MissionDone); //替换为MissionDone的监听，但增加了Error
                report = new MissionDone
                {
                    Error = (report as MissionFail)?.Log
                };
            }

            #endregion
            var tempReport = new AgvReport(report.MRID, report.MissionID, reportType, null);
            var received = true; //默认agree = true
            var key = tempReport.GetKey();
            if (Watchs.TryGetValue(key, out var agvReport))
            {
                agvReport.Report = report;
                agvReport.Ms = DateTime.Now.Subtract(agvReport.CreateTime).TotalMilliseconds;
                agvReport.WaitHandle?.Set(); // 发送信号
                received = agvReport.AgreeCall?.Invoke(report) ?? true;
            }
            else
            {
                //IM Report是不受控制的。
                //有可能这里还没有AddWatch,通知却已经提前到达了。
                tempReport.Report = report;
                TryAddWatch(tempReport);
                Console.Error.WriteLine($"<<预加入>>{report.MRID}->{key}");
            }

            return report.GetResponse(received);
        }

        public BaseReport GetReport(string key)
        {
            if (Watchs.TryGetValue(key, out var wc))
            {
                return wc.Report;
            }

            return null;
        }
    }
}