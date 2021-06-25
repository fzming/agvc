
using System;
using System.Text;
using System.Threading.Tasks;
using AgvcAgent.Api.Filters.GlobalFilters;
using CoreService.JwtToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

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
            services.AddControllers(configure =>
            { 
                configure.Filters.Add(typeof(ApiActionFilter));
                configure.Filters.Add(typeof(ApiExceptionFilter));
                configure.RespectBrowserAcceptHeader = true;
                configure.EnableEndpointRouting = false;
            }).AddNewtonsoftJson(options => options.UseMemberCasing());
            // services.AddControllers();
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication1", Version = "v1" });
            // });

            #region JWT CONFIG

            services.Configure<JwtTokenOptions>(Configuration.GetSection("JwtTokenOptions"));
            JwtTokenOptions tokenOptions = Configuration.GetSection("JwtTokenOptions").Get<JwtTokenOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //开启Bearer服务认证
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = tokenOptions.ToTokenValidationParams();
                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new MyTokenValidator());
                });

            #endregion

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseSwagger();
                // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1"));
            }


            app.UseRouting();
            //启用跨域
            app.UseCors("cors");
            //引入wwwroot
            app.UseStaticFiles();
            app.UseAuthentication();
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
