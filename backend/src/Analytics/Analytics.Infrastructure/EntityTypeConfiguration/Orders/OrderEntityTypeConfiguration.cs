using Analytics.Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValueObjects.ValueObject;

namespace Analytics.Infrastructure.EntityTypeConfiguration.Orders;

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", "Analytics");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.TenantId).IsRequired();

        builder.Property(o => o.CustomerId).IsRequired();

        builder.Property(o => o.ExternalOrderId)
            .HasMaxLength(100);

        builder.Property(o => o.OrderDate).IsRequired();
        builder.Property(o => o.CreatedAt).IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.ComplexProperty(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasConversion(
                    currency => currency.Code,
                    code => new Currency(code))
                .HasMaxLength(3) 
                .IsRequired();
        });
        
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(Order.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore("ComputeItemsSum");
        builder.Ignore("ComputeGrossMargin");

        builder.HasIndex(o => new { o.TenantId, o.ExternalOrderId })
            .IsUnique()
            .HasDatabaseName("IX_Orders_Tenant_ExternalId");

        builder.HasIndex(o => new { o.TenantId, o.OrderDate })
            .HasDatabaseName("IX_Orders_Tenant_OrderDate");

        builder.HasIndex(o => new { o.TenantId, o.CustomerId, o.OrderDate })
            .HasDatabaseName("IX_Orders_Tenant_Customer_Date");

        builder.HasIndex(o => new { o.TenantId, o.Status, o.OrderDate })
            .HasDatabaseName("IX_Orders_Tenant_Status_Date");
    }
}