using System;
using System.Threading.Tasks;
using Messages.Parser;
using Microsoft.AspNetCore.Mvc;
using Protocol;
using Protocol.Query;
using RobotFactory;
using Serialize;

namespace AgvcAgent.Api
{

    [ApiController]
    [Route("IMServer")]
    public class TestApi : ControllerBase
    {
        [Route("tx501i")]
        public string tx501i(string mrid)
        {
            //測試任務
            var mqMessage =
                "TX501I                      001BL$WMS202                                        BL        N    A               LKXLJBT01 01        DJSLJBT01 01                            10105114601764                  ";

            var message = MessageParser.Parse(mqMessage);
            AgvcCenter.Instance.TaskEngine.AcceptMessage(message, mrid);
            return mqMessage;
        }
        /// <summary>
        /// 測試用，同意 iM 所有請求及回報。呼叫方式：http://localhost:5001/IMServer/AllwaysTrue?json= 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpGet, Route("AllwaysTrue")]
        public string AllwaysTrue([FromQuery] string json)
        {
            //Console.WriteLine(">>" + json);
             
            var o = json?.DeserializeJsonToObject();
            var serializeJson = o switch
            {
                BaseReport report => AgvReporter.Instance.OnReport(report).SerializeJSONObject(),
                BaseRequest request => request.GetResponse(true, "Allways True").SerializeJSONObject(),
                Echo echo => echo.GetResponse().SerializeJSONObject(),
                _ => string.Empty
            };
           // Console.WriteLine("<< " + str);
            return serializeJson;
        }


    }
}