using System;
using System.Collections.Concurrent;
using AgvcWorkFactory.Interfaces;
using Protocol;
using Protocol.Report;

namespace AgvcWorkFactory
{
    /// <summary>
    ///     AGV设备回调监控
    /// </summary>
    public class AgvReporter : IAgvReporter
    {
        private readonly ConcurrentDictionary<string, AgvReport> Watchs =
            new();

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
        ///     IMG->AGVC 汇报了状态
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public Response OnReport(BaseReport report)
        {
            Console.WriteLine($"<<Report>> {report.MRID}->{report.Type.FullName}");
            var reportType = report.Type; //当前报告的任务类别

            #region 针对MissionFail特殊处理

            /*Mission Done 和 Mission Fail是互斥的,不会同时在一个任务中出现*/
            if (reportType == typeof(MissionFail)) //agv报告了MissionFail
            {
                reportType = typeof(MissionDone); //替换为MissionDone的监听，但增加了Error
                report = new MissionDone
                {
                    Error = (report as MissionFail)?.Log
                };
            }

            #endregion

            var tempReport = new AgvReport(report.MRID, report.MissionID, reportType, null);
            var received = true; //默认agree = true,该参数表明AGVC已经成功收到汇报,将在本次请求中同步返回IM.
            var key = tempReport.GetKey();
            //当前监控列表中存在此任务的监控
            if (Watchs.TryGetValue(key, out var agvReport))
            {
                agvReport.Report = report;
                agvReport.Ms = DateTime.Now.Subtract(agvReport.CreateTime).TotalMilliseconds;
                agvReport.WaitHandle?.Set(); // 发送信号,任务线程不再阻塞,将继续执行下一个Mission Step.
                received = agvReport.AgreeCall?.Invoke(report) ?? true;
            }
            else
            {
                //!!注意!! --- IM Report是不受控制的。
                //有可能这里还没有AddWatch,通知却已经提前到达了。
                tempReport.Report = report;
                TryAddWatch(tempReport); //尝试预先加入到监控中,伪处理.
                Console.Error.WriteLine($"<<预加入>>{report.MRID}->{key}");
            }

            return report.GetResponse(received);
        }
        
        public BaseReport GetReport(string key)
        {
            if (Watchs.TryGetValue(key, out var wc)) return wc.Report;

            return null;
        }
    }
}