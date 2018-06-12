using BookingService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace BookingService.Data.EntityConfigurations
{
    public class BookingEntityTypeConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("bookings");            

            builder.HasKey(ci => ci.Id);

            builder.Property<DateTime>("Date").IsRequired();
            builder.Property<string>("UserId").IsRequired(false);
            builder.Property<int>("StatusId").IsRequired();            

            builder.HasOne(o => o.Status)
                .WithMany()
                .HasForeignKey("StatusId");

            builder.Property<int>("ProductId")
                .IsRequired();

            builder.Property<string>("ProductName")
                .IsRequired();

            builder.Property<decimal>("UnitPrice")
                .IsRequired();

            builder.Property<int>("Units")
                .IsRequired();
        }
    }
}
