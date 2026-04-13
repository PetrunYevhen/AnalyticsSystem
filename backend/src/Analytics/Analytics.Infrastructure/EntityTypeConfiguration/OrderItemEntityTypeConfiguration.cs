using Analytics.Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Analytics.Infrastructure.EntityTypeConfiguration;

public class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.ProductExternalId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(oi => oi.ProductName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(oi => oi.Category)
            .HasMaxLength(100);

        builder.Property(oi => oi.UnitPrice)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(oi => oi.UnitCost)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(oi => oi.Quantity)
            .IsRequired();
            
        builder.HasIndex(oi => oi.ProductExternalId);
    }
}