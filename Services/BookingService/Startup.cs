using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BookingService.Data;
using BookingService.Infrastructure;
using BookingService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Polly;

namespace BookingService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddEntityFrameworkNpgsql().AddDbContext<BookingContext>(options =>
                options.UseNpgsql(connectionString,
                    sql =>
                    {
                        sql.MigrationsAssembly(migrationsAssembly);
                        sql.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                    }));

            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters()
                .AddControllersAsServices();

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = identityUrl;
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "booking";
                });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            
            containerBuilder.RegisterModule(new ApplicationModule());

            var container = RegisterEventBus(containerBuilder);

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseMvc();

            WaitForSqlAvailabilityAsync(loggerFactory, app, env).Wait();
        }

        IContainer RegisterEventBus(ContainerBuilder containerBuilder)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            EnsurePostgreSqlDatabaseExistsAsync(connectionString).Wait();



            var container = containerBuilder.Build();



            return container;
        }

        async Task EnsurePostgreSqlDatabaseExistsAsync(string connectionString)
        {
            var retry = Policy.Handle<SocketException>()
                         .WaitAndRetryAsync(new TimeSpan[]
                         {
                             TimeSpan.FromSeconds(5),
                             TimeSpan.FromSeconds(10),
                             TimeSpan.FromSeconds(15),
                         });

            await retry.ExecuteAsync(async () =>
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                var originalDatabase = builder.Database;

                builder.Database = "postgres";
                var masterConnectionString = builder.ConnectionString;

                bool dbExists;
                using (var connection = new NpgsqlConnection(masterConnectionString))
                {
                    await connection.OpenAsync();
                    var command = connection.CreateCommand();
                    command.CommandText = $"SELECT 1 FROM pg_catalog.pg_database WHERE datname = '{originalDatabase}'";

                    var result = await command.ExecuteScalarAsync();

                    dbExists = result != null;
                }

                if (!dbExists)
                {
                    using (var connection = new NpgsqlConnection(masterConnectionString))
                    {
                        await connection.OpenAsync();
                        var command = connection.CreateCommand();
                        command.CommandText = $@"CREATE DATABASE ""{originalDatabase}""";
                        await command.ExecuteNonQueryAsync();
                    }
                }
            });
        }

        private async Task WaitForSqlAvailabilityAsync(ILoggerFactory loggerFactory, IApplicationBuilder app, IHostingEnvironment env, int retries = 0)
        {
            var logger = loggerFactory.CreateLogger(nameof(Startup));
            var policy = CreatePolicy(retries, logger, nameof(WaitForSqlAvailabilityAsync));
            await policy.ExecuteAsync(async () =>
            {
                var context = (BookingContext)app
                    .ApplicationServices.GetService(typeof(BookingContext));

                await BookingContextSeed.SeedAsync(context);
            });

        }

        private Policy CreatePolicy(int retries, ILogger logger, string prefix)
        {
            return Policy.Handle<SocketException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }
    }
}
