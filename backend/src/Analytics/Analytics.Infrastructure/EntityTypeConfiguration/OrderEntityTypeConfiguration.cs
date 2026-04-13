using Analytics.Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Analytics.Infrastructure.EntityTypeConfiguration;

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.ExternalOrderId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("numeric(18,2)"); 

        builder.Property(o => o.Currency)
            .IsRequired()
            .HasMaxLength(3); 
        
        builder.Property(prop => prop.Status).HasConversion<string>();
        
        builder.Property(o => o.CustomAttributesJson)
            .HasColumnType("jsonb");

        builder.HasMany(o => o.Items)
            .WithOne() 
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(o => o.TenantId);
        builder.HasIndex(o => o.CustomerId);
    }
}