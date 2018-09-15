using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGateway
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
            var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            var authenticationProviderKey = "IdentityApiKey";

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddAuthentication()
               .AddJwtBearer(authenticationProviderKey, x =>
               {
                   x.Authority = identityUrl;
                   x.RequireHttpsMetadata = false;
                   x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                   {
                       ValidAudiences = new[] { "products", "booking", "notification", "bookingagg" }
                   };
                   x.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents()
                   {
                       OnAuthenticationFailed = async ctx =>
                       {                           
                       },
                       OnTokenValidated = async ctx =>
                       {                           
                       },
                       OnMessageReceived = async ctx =>
                       {                           
                       }
                   };
               });

            services.AddOcelot(Configuration);
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

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseOcelot().Wait();
        }
    }
}
