using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AgvcAgent
{
    class Program
    {

        static void Main(string[] args)
        {
            var agvc = AgvcCenter.Instance;
            agvc.Run();
            CreateWebHostBuilder(args).Build().Run();
            agvc.Stop();

        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration(builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                })
                .UseUrls("http://*:5000;http://localhost:5001;")
                .ConfigureKestrel((context, options) =>
                {
                    options.Limits.MaxRequestBodySize = 20000000;
                })
                .ConfigureLogging(logging =>
                {
                    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0
                    logging.ClearProviders();
                    //logging.AddConsole();
                })
                .UseStartup<Startup>();
    }
}
