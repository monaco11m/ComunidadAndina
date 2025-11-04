using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

            builder.Property(x => x.CustomerName)
            .IsRequired()
            .HasMaxLength(50);

            builder.Property(x => x.CustomerEmail)
            .IsRequired()
            .HasMaxLength(50);

            builder.Property(x => x.OrderDate)
            .IsRequired();

            builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

            builder.Property(x => x.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

            builder.HasMany(x => x.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
