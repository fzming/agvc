using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using AgvcWorkFactory.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AgvcAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            DependencyInjection.ServiceProvider = webHost.Services;
            var agvc = DependencyInjection.GetService<IAgvcCenter>();
            agvc.Run();
            webHost.Run();
            agvc.Stop();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration(builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                })
                .UseUrls("http://*:5000;http://localhost:5001;")
                .ConfigureServices(DependencyInjection.ConfigureServices)
                .ConfigureKestrel((context, options) =>
                {
                    options.Limits.MaxRequestBodySize = 20000000;
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                   // var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                    logging.ClearProviders();//去掉默认添加的日志提供程序
                    // IMPORTANT: This needs to be added *before* configuration is loaded, this lets
                    // the defaults be overridden by the configuration.
                    // if (isWindows)
                    // {
                    //     // Default the EventLogLoggerProvider to warning or above
                    //     logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
                    // }

                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
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
