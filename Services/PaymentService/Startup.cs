using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using MicroStore.Extensions.HealthChecks;

namespace PaymentService
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
                checks.AddNpgsqlCheck("paymentdb", connectionString, TimeSpan.FromMinutes(1));
            });

            services.AddMvc();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);

            var container = RegisterEventBus(containerBuilder, services);

            return new AutofacServiceProvider(container);
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

            app.UseMvc();
        }

        IContainer RegisterEventBus(ContainerBuilder containerBuilder, IServiceCollection services)
        {
            var useAzureServiceBus = Configuration.GetValue<bool>("AzureServiceBusEnabled");

            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            EnsurePostgreSqlDatabaseExistsAsync(connectionString).Wait();

            if (!useAzureServiceBus)
                EnsureRabbitConnectionExists(services);

            IEndpointInstance endpoint = null;
            containerBuilder.Register(c => endpoint)
                .As<IEndpointInstance>()
                .SingleInstance();

            var container = containerBuilder.Build();

            var endpointConfiguration = new EndpointConfiguration("Payment");

            if (useAzureServiceBus)
            {
                // Configure Azure Service Bus transport
                var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
                var connStr = Configuration["EventBusConnection"];
                transport.ConnectionString(connStr);
            }
            else
            {
                // Configure RabbitMQ transport
                var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                transport.UseConventionalRoutingTopology();
                transport.ConnectionString(GetRabbitConnectionString());
            }            

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
    }
}
