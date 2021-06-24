using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using AgvcRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RobotFactory.Interfaces;

namespace AgvcAgent
{
    class Program
    {


        static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            DependencyInjection.ServiceProvider = webHost.Services;
            var agvc = DependencyInjection.GetService<IAgvcCenter>();
            var mrrepo = DependencyInjection.GetService<IMrRepository>();

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
                .ConfigureLogging(logging =>
                {
                    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0
                    logging.ClearProviders();
                    //logging.AddConsole();
                })
                .UseStartup<Startup>();
    }
}
