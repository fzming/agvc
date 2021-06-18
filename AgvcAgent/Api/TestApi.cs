using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AgvcAgent.Api
{

    [ApiController]
    [Route("test")]
    public class TestApi:ControllerBase
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
    }
}