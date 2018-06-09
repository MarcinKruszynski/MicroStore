using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Data
{
    public class ProductContextSeed
    {
        public static async Task SeedAsync(ProductContext context)
        {
            context.Database.Migrate();

            if (!context.ProductItems.Any())
            {
                await context.ProductItems.AddRangeAsync(Config.GetProductItems());

                await context.SaveChangesAsync();                    
            }                                
        }        
    }
}
