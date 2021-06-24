
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AgvcAgent
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // services.Configure<TimedExecuteServiceSettings>(Configuration.GetSection("TCS"));
            // services.AddSingleton<IHostedService, TimedExecuteService>();
            //  services.AddMvc(options => { options.EnableEndpointRouting = false; });//注册MVC服务，启用MVC应用程序模型，
            services.AddControllers(configure =>
            {
                configure.EnableEndpointRouting = false;
            }).AddNewtonsoftJson(options => options.UseMemberCasing());
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseMvc(s =>
            {
                s.MapRoute("default", "{controller}/{action}/{id?}");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            // app.Run(context =>
            // {
            //     return context.Response.WriteAsync("Hello world");
            // });
        }
    }
}
