using System;
using System.Threading.Tasks;
using AgvcAgent.Api.Filters.GlobalFilters;
using AgvcRepository;
using AgvcWorkFactory.Interfaces;
using CoreRepository;
using Messages.Parser;
using Microsoft.AspNetCore.Mvc;
using Protocol;
using Protocol.Query;
using Serialize;

namespace AgvcAgent.Api
{

    [ApiController]
    [Route("agvc")]
    public class AgvcApi : ControllerBase
    {
        private IRobotTaskEngine TaskEngine { get; }
        private IAgvReporter AgvReporter { get; }
        private IMessageSerializer MessageParser { get; }

        public AgvcApi(IRobotTaskEngine taskEngine, IAgvReporter agvReporter,IMessageSerializer messageParser)
        {
            TaskEngine = taskEngine;
            AgvReporter = agvReporter;
            MessageParser = messageParser;
        }

        [Route("test1"),IgnoreResultModel]
        public string test()
        {
            return "Abc";
        }
        [Route("test2")]
        public string test2()
        {
            return "Abc";
        }
        [Route("tx501i")]
        public string tx501i(string mrid)
        {
            //測試任務
            var mqMessage =
                "TX501I                      001BL$WMS202                                        BL        N    A               LKXLJBT01 01        DJSLJBT01 01                            10105114601764                  ";

            var message = MessageParser.Deserialize(mqMessage);
            TaskEngine.AcceptMessage(message, mrid);
            return mqMessage;
        }
        
        /// <summary>
        /// 同意 iM 所有請求及回報。呼叫方式：http://localhost:5001/agvc/request?json= 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpGet, Route("request"),IgnoreResultModel]
        public string ImRequest([FromQuery] string json)
        {
            //Console.WriteLine(">>" + json);
             
            var o = json?.DeserializeJsonToObject();
            var serializeJson = o switch
            {
                BaseReport report => AgvReporter.OnReport(report).SerializeJSONObject(),
                BaseRequest request => request.GetResponse(true, "Allways True").SerializeJSONObject(),
                Echo echo => echo.GetResponse().SerializeJSONObject(),
                _ => string.Empty
            };
           // Console.WriteLine("<< " + str);
            return serializeJson;
        }


    }
}