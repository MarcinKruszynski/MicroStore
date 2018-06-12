using BookingService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Data.EntityConfigurations
{
    public class BuyerEntityTypeConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.ToTable("buyers");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.IdentityGuid)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex("IdentityGuid")
              .IsUnique(true);
        }
    }
}
