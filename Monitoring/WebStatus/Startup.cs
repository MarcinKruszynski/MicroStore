using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebStatus.Extensions;

namespace WebStatus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // Add framework services.
            services.AddHealthChecks(checks =>
            {
                var minutes = 1;

                checks.AddUrlCheckIfNotNull(Configuration["IdentityUrl"], TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheckIfNotNull(Configuration["ProductUrl"], TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheckIfNotNull(Configuration["BookingUrl"], TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheckIfNotNull(Configuration["PaymentUrl"], TimeSpan.FromMinutes(minutes));
                checks.AddUrlCheckIfNotNull(Configuration["NotificationUrl"], TimeSpan.FromMinutes(minutes));                               
                checks.AddUrlCheckIfNotNull(Configuration["spa"], TimeSpan.Zero); //No cache for this HealthCheck, better just for demos 
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            app.Map("/liveness", lapp => lapp.Run(async ctx => ctx.Response.StatusCode = 200));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
