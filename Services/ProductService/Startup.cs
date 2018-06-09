using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using ProductService.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using NpgsqlTypes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using System.Net.Sockets;

namespace ProductService
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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddEntityFrameworkNpgsql().AddDbContext<ProductContext>(options =>
                options.UseNpgsql(connectionString,
                    sql =>
                    {
                        sql.MigrationsAssembly(migrationsAssembly);
                        sql.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                    }));


            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = identityUrl;
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "products";
                });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });


            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(services);
            
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

        private IContainer RegisterEventBus(ContainerBuilder containerBuilder)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            EnsurePostgreSqlDatabaseExists(connectionString);

            IEndpointInstance endpoint = null;
            containerBuilder.Register(c => endpoint)
                .As<IEndpointInstance>()
                .SingleInstance();

            var container = containerBuilder.Build();

            var endpointConfiguration = new EndpointConfiguration("Products");

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

        void EnsurePostgreSqlDatabaseExists(string connectionString)
        {            
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var originalDatabase = builder.Database;

            builder.Database = "postgres";
            var masterConnectionString = builder.ConnectionString;

            bool dbExists;
            using (var connection = new NpgsqlConnection(masterConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT 1 FROM pg_catalog.pg_database WHERE datname = '{originalDatabase}'";

                dbExists = command.ExecuteScalar() != null;
            }

            if (!dbExists)
            {
                using (var connection = new NpgsqlConnection(masterConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $@"CREATE DATABASE ""{originalDatabase}""";
                    command.ExecuteNonQuery();
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
                var context = (ProductContext)app
                    .ApplicationServices.GetService(typeof(ProductContext));                

                await ProductContextSeed.SeedAsync(context);
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
