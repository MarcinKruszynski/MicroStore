using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BookingService.Data;
using BookingService.Infrastructure;
using BookingService.Infrastructure.Filters;
using BookingService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Common;
using Swashbuckle.AspNetCore.Swagger;
using MicroStore.Extensions.HealthChecks;

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

            services.AddHealthChecks(checks =>
            {
                checks.AddNpgsqlCheck("bookingdb", connectionString, TimeSpan.FromMinutes(1));
            });

            services.AddMvc()
                .AddControllersAsServices();
            
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddEntityFrameworkNpgsql().AddDbContext<BookingContext>(options =>
                options.UseNpgsql(connectionString,
                    sql =>
                    {
                        sql.MigrationsAssembly(migrationsAssembly);
                        sql.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                    }));

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "MicroStore - Booking HTTP API",
                    Version = "v1",
                    Description = "The Booking Microservice HTTP API.",
                    TermsOfService = "Terms Of Service"
                });

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize",
                    TokenUrl = $"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/token",
                    Scopes = new Dictionary<string, string>()
                    {
                        { "booking", "Booking API" }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = identityUrl;
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "booking";
                });

            services.AddOptions();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            
            containerBuilder.RegisterModule(new ApplicationModule());

            var container = RegisterEventBus(containerBuilder, services);

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
              .UseSwaggerUI(c =>
              {
                  c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "BookingService V1");
                  c.OAuthClientId("bookingswaggerui");
                  c.OAuthAppName("Booking Swagger UI");
              });

            WaitForSqlAvailabilityAsync(loggerFactory, app, env).Wait();
        }

        IContainer RegisterEventBus(ContainerBuilder containerBuilder, IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            EnsurePostgreSqlDatabaseExistsAsync(connectionString).Wait();

            EnsureRabbitConnectionExists(services);

            IEndpointInstance endpoint = null;
            containerBuilder.Register(c => endpoint)
                .As<IEndpointInstance>()
                .SingleInstance();

            var container = containerBuilder.Build();

            var endpointConfiguration = new EndpointConfiguration("Booking");

            // Configure RabbitMQ transport
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            transport.ConnectionString(GetRabbitConnectionString());

            // Configure persistence
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var dialect = persistence.SqlDialect<SqlDialect.PostgreSql>();
            dialect.JsonBParameterModifier(
            modifier: parameter =>
            {
                var npgsqlParameter = (NpgsqlParameter)parameter;
                npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
            });
            persistence.ConnectionBuilder(connectionBuilder:
                () => new NpgsqlConnection(connectionString));

            // Use JSON.NET serializer
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

            // Enable the Outbox.
            endpointConfiguration.EnableOutbox();

            // Make sure NServiceBus creates queues in RabbitMQ, tables in SQL Server, etc.
            // You might want to turn this off in production, so that DevOps can use scripts to create these.
            endpointConfiguration.EnableInstallers();

            // Turn on auditing.
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            // Define conventions
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(c => c.Namespace != null && c.Name.EndsWith("IntegrationEvent"));

            // Configure the DI container.
            endpointConfiguration.UseContainer<AutofacBuilder>(customizations: customizations =>
            {
                customizations.ExistingLifetimeScope(container);
            });

            // Start the endpoint and register it with ASP.NET Core DI
            endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

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

        void EnsureRabbitConnectionExists(IServiceCollection services)
        {
            var factory = new ConnectionFactory()
            {
                HostName = Configuration["EventBusConnection"]
            };

            if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
            {
                factory.UserName = Configuration["EventBusUserName"];
            }

            if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
            {
                factory.Password = Configuration["EventBusPassword"];
            }

            var retryCount = 5;

            var scopeFactory = services
                    .BuildServiceProvider()
                    .GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var provider = scope.ServiceProvider;

                var logger = provider.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                using (var persistentConnection = new DefaultRabbitMQPersistentConnection(factory, logger, retryCount))
                {
                    persistentConnection.TryConnect();
                }
            }
        }

        private string GetRabbitConnectionString()
        {
            var host = Configuration["EventBusConnection"];
            var user = Configuration["EventBusUserName"];
            var password = Configuration["EventBusPassword"];

            if (string.IsNullOrEmpty(user))
                return $"host={host}";

            return $"host={host};username={user};password={password};";
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
