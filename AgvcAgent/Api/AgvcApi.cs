using System.Collections.Generic;
using AgvcAgent.Api.Filters.GlobalFilters;
using AgvcWorkFactory.Interfaces;
using AgvcWorkFactory.Tasks;
using Messages.Serializer;
using Messages.Transfers.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Protocol;
using Protocol.Query;
using Serialize;

namespace AgvcAgent.Api
{
    [ApiController]
    [Route("agvc")]
    public class AgvcApi : ControllerBase
    {
        public AgvcApi(IRobotTaskEngine taskEngine, IAgvReporter agvReporter,IAgvRequester agvRequester, IMessageSerializer messageParser)
        {
            TaskEngine = taskEngine;
            AgvReporter = agvReporter;
            AgvRequester = agvRequester;
            MessageParser = messageParser;
        }

        private IRobotTaskEngine TaskEngine { get; }
        private IAgvReporter AgvReporter { get; }
        public IAgvRequester AgvRequester { get; }
        private IMessageSerializer MessageParser { get; }

        [Route("test1")]
        [SkipActionFilter]
        public string test()
        {
            return "Abc";
        }
        /// <summary>
        /// 测试人工任务
        /// </summary>
        /// <param name="file"></param>
        /// <param name="mrid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("task")]
        public bool testFromTo(string file, string mrid)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile($"{file}.json");
            var configuration = builder.Build();
            var froms = configuration.GetSection("from").Get<List<TaskGoal>>();
            var tos = configuration.GetSection("to").Get<List<TaskGoal>>();

            var userTask = new UserTask()
            {
                MRID = mrid,
                Froms = froms,
                Tos = tos,
            };
            TaskEngine.AcceptUserTask(userTask);
            return true;
        }

        [Route("tx501i")]
        public IMessage tx501i(string mrid)
        {
            //測試任務
            var mqMessage =
                "TX501I                      001BL$WMS202                                        BL        N    A               LKXLJBT01 01        DJSLJBT01 01                            10105114601764                  ";

            var message = MessageParser.Deserialize(mqMessage);
            TaskEngine.AcceptMessage(message, mrid);
            return message;
        }

        /// <summary>
        ///     同意 iM 所有請求及回報。呼叫方式：http://localhost:5001/agvc/report?json=
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("report")]
        [SkipActionFilter]
        public string ImRequest([FromQuery] string json)
        {
            //Console.WriteLine(">>" + json);

            var o = json?.DeserializeJsonToObject();
            var serializeJson = o switch
            {
                BaseReport report => AgvReporter.OnReport(report).SerializeJSONObject(),
                BaseRequest request => AgvRequester.OnRequest(request).SerializeJSONObject(),
                Echo echo => echo.GetResponse().SerializeJSONObject(),
                _ => string.Empty
            };
            // Console.WriteLine("<< " + str);
            return serializeJson;
        }
    }
}