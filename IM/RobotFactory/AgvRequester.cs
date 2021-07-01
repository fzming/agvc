using System;
using System.Collections.Concurrent;
using AgvcWorkFactory.Interfaces;
using Microsoft.Extensions.Configuration;
using Protocol;
using Protocol.Request;

namespace AgvcWorkFactory
{
    /// <summary>
    ///     AGV设备请求监控
    /// </summary>
    public class AgvRequester : IAgvRequester
    {
        private IAgvcConfiguration AgvcConfiguration { get; }
        private readonly ConcurrentDictionary<string, AgvRequest> Watchs =
            new();

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AgvRequester(IAgvcConfiguration agvcConfiguration)
        {
            AgvcConfiguration = agvcConfiguration;
        }

        public bool TryAddWatch(AgvRequest agvReport)
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

        public int GetAgvInitializeInterval()
        {
            return AgvcConfiguration.GetConfig().InitializeCheckInterval;
        }

        /// <summary>
        ///     IM->AGVC 请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Response OnRequest(BaseRequest request)
        {
            Console.WriteLine($"<<Request>> {request.MRID}->{request.Type.FullName}");
            var requestType = request.Type; //当前请求的任务类别

            var tempRequest = new AgvRequest(request.MRID, request.MissionID, requestType, null);
            var received = true; //默认agree = true,该参数表明AGVC已经成功收到汇报,将在本次请求中同步返回IM.
            var key = tempRequest.GetKey();
            //当前监控列表中存在此任务的监控
            if (Watchs.TryGetValue(key, out var agvRequest))
            {
                agvRequest.Request = request;
                agvRequest.Ms = DateTime.Now.Subtract(agvRequest.CreateTime).TotalMilliseconds;
                agvRequest.WaitHandle?.Set(); // 发送信号,任务线程不再阻塞,将继续执行下一个Mission Step.
                received = agvRequest.AgreeCall?.Invoke(request) ?? true;
            }
            else
            {
                //!!注意!! --- IM Request是不受控制的。
                //有可能这里还没有AddWatch,通知却已经提前到达了。
                tempRequest.Request = request;
                TryAddWatch(tempRequest); //尝试预先加入到监控中,伪处理.
                Console.Error.WriteLine($"<<预加入>>{request.MRID}->{key}");
            }

            return request.GetResponse(received,"LOG NOT DEFINE");
        }
        
        public BaseRequest GetRequest(string key)
        {
            if (Watchs.TryGetValue(key, out var wc)) return wc.Request;

            return null;
        }
    }
}