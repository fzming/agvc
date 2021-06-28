using System;
using System.IO;
using AgvcWorkFactory.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AgvcAgent
{
    internal class Program
    {
        private static IConfiguration _configuration;
        private static string urls;

        private static void Main(string[] args)
        {
            Console.Title = "AgvcAgentV1.0";
            //Console.ForegroundColor = ConsoleColor.Green;

            _configuration = CreateConfiguration();
            var agvConfig = _configuration.GetSection("AGVC").Get<AgvcConfig>();
            urls = agvConfig?.ListenUrls;
            var webHostBuilder = CreateWebHostBuilder(args);
            if (webHostBuilder==null)
            {
                throw new Exception("webHostBuilder Create Failed");
            }

            IWebHost webHost = null;
            try
            {
                webHost =webHostBuilder.Build();
                DependencyInjection.ServiceProvider = webHost.Services;
            }
            catch (Exception e)
            {
                Console.WriteLine("webHostBuilder.Build() Failed:");
                Console.WriteLine(e);
            }
           
            var agvc = DependencyInjection.GetService<IAgvcCenter>();
            if (agvc==null)
            {
                throw new Exception("Fail to Create IAgvcCenter");
            }
            agvc.Run();
            webHost?.Run();
            agvc.Stop();
        }

        static IConfiguration CreateConfiguration()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile("hosting.json", optional: true)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //.AddCommandLine(args)
                //.AddEnvironmentVariables()
                .Build();
            return config;
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseConfiguration(_configuration)
                .UseUrls(urls)
                //.UseUrls("http://*:5200;http://localhost:5300;")
                .ConfigureServices(DependencyInjection.ConfigureServices)
                .ConfigureKestrel((context, options) =>
                {
                    options.Limits.MaxRequestBodySize = 20000000;
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                    logging.ClearProviders(); //去掉默认添加的日志提供程序
                    // IMPORTANT: This needs to be added *before* configuration is loaded, this lets
                    // the defaults be overridden by the configuration.
                    // if (isWindows)
                    // {
                    //     // Default the EventLogLoggerProvider to warning or above
                    //     logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
                    // }

                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    //logging.AddDebug();
                    //  logging.AddEventSourceLogger();

                    // if (isWindows)
                    // {
                    //     // Add the EventLogLoggerProvider on windows machines
                    //     //logging.AddEventLog();
                    // }
                })
                .UseStartup<Startup>();
        }
    }
}