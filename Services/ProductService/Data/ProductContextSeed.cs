using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Data
{
    public class ProductContextSeed
    {
        public async Task SeedAsync(ProductContext context, ILogger<ProductContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(ProductContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                if (!context.ProductItems.Any())
                {
                    await context.ProductItems.AddRangeAsync(Config.GetProductItems());

                    await context.SaveChangesAsync();                    
                }
            });                    
        }

        private Policy CreatePolicy(ILogger<ProductContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<NpgsqlException>().
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
