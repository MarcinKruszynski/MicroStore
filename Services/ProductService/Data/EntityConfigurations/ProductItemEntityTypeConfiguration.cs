using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Model;

namespace ProductService.Data.EntityConfigurations
{
    public class ProductItemEntityTypeConfiguration: IEntityTypeConfiguration<ProductItem>
    {
        public void Configure(EntityTypeBuilder<ProductItem> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(ci => ci.Id);                

            builder.Property(ci => ci.Name)
                .IsRequired(true)
                .HasMaxLength(255);

            builder.Property(ci => ci.Price)
                .IsRequired(true);            
        }
    }    
}
