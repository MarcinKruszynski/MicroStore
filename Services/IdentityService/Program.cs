using System.IO;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityService.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<PersistedGrantDbContext>((_, __) => { })
                .MigrateDbContext<ApplicationDbContext>((_, __) => { }) 
                .MigrateDbContext<ConfigurationDbContext>((context, services) =>
                {
                    var configuration = services.GetService<IConfiguration>();

                    new ConfigurationDbContextSeed()
                        .SeedAsync(context, configuration)
                        .Wait();
                }).Run();            
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ic => ic.AddJsonFile(Path.Combine("cert", "configuration.json")))
                .UseStartup<Startup>()
                .Build();
    }
}
