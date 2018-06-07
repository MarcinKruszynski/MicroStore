using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductService.Data;

namespace ProductService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)                
                .MigrateDbContext<ProductContext>((context, services) =>
                {
                    var logger = services.GetService<ILogger<ProductContextSeed>>();

                    new ProductContextSeed()
                        .SeedAsync(context, logger)
                        .Wait();
                }).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
