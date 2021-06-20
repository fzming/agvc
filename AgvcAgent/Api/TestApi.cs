using System;
using System.Threading.Tasks;
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
        [Route("index")]
        public ActionResult<string> Index()
        {
            Console.WriteLine($"DateTime.Now={DateTime.Now}");
            return "hello api";
        }
        [Route("indexTask")]
        public Task<string> Index2()
        {
            Console.WriteLine($"DateTime.Now={DateTime.Now}");
            return Task.FromResult("hello api from async task");
        }
        /// <summary>
        /// 測試用，同意 iM 所有請求及回報。呼叫方式：http://localhost:5001/IMServer/AllwaysTrue?json= 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpGet, Route("AllwaysTrue")]
        public string AllwaysTrue([FromQuery] string json)
        {
            Console.WriteLine(">>" + json);
             
            object obj2 = json?.DeserializeJsonToObject();
            string str = obj2 switch
            {
                BaseReport report => IMReporter.OnReport(report).SerializeJSONObject(),
                BaseRequest request => request.GetResponse(true, "Allways True").SerializeJSONObject(),
                Echo echo => echo.GetResponse().SerializeJSONObject(),
                _ => string.Empty
            };
            Console.WriteLine("<< " + str);
            return str;
        }


    }
}