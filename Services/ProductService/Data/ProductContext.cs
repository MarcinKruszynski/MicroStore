using Microsoft.EntityFrameworkCore;
using ProductService.Data.EntityConfigurations;
using ProductService.Model;

namespace ProductService.Data
{
    public class ProductContext: DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }
        public DbSet<ProductItem> ProductItems { get; set; }        

        protected override void OnModelCreating(ModelBuilder builder)
        {            
            builder.ApplyConfiguration(new ProductItemEntityTypeConfiguration());
        }
    }
}
