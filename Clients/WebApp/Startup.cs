using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.HealthChecks;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks(checks =>
            {
                checks.AddUrlCheck(Configuration["IdentityUrlHC"], TimeSpan.FromMinutes(1));
                checks.AddUrlCheck(Configuration["ProductUrlHC"], TimeSpan.FromMinutes(1));
                checks.AddUrlCheck(Configuration["BookingUrlHC"], TimeSpan.FromMinutes(1));
                checks.AddUrlCheck(Configuration["PaymentUrlHC"], TimeSpan.FromMinutes(1));
                checks.AddUrlCheck(Configuration["NotificationUrlHC"], TimeSpan.FromMinutes(1));
            });

            services.Configure<AppSettings>(Configuration);

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                //app.UsePathBase(pathBase);

                app.Use(async (context, next) =>
                {
                    context.Request.PathBase = pathBase;
                    await next.Invoke();
                });
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}
