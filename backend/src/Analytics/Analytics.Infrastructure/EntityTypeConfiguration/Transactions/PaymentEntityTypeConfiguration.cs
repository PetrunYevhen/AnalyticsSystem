using Analytics.Domain.Entities.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValueObjects.ValueObject;

namespace Analytics.Infrastructure.EntityTypeConfiguration.Transactions;

public class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        
        builder.ToTable("Payments", "Analytics");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.TenantId).IsRequired();
        builder.Property(p => p.OrderId).IsRequired();
        builder.Property(p => p.Status).IsRequired();
        builder.Property(p => p.Status).HasConversion<string>().IsRequired();
        
        builder.ComplexProperty(p => p.TotalAmount, m =>
        {
            m.Property(x => x.Amount).HasColumnName("TotalAmount").HasColumnType("numeric(18,2)");
            m.Property(x => x.Currency).HasColumnName("TotalCurrency").HasMaxLength(3)
                .HasConversion(c => c.Code, code => Currency.Parse(code));
        });

        builder.ComplexProperty(p => p.PaidAmount, m =>
        {
            m.Property(x => x.Amount).HasColumnName("PaidAmount").HasColumnType("numeric(18,2)");
            m.Property(x => x.Currency).HasColumnName("PaidCurrency").HasMaxLength(3)
                .HasConversion(c => c.Code, code => Currency.Parse(code));
        });
        
        builder.HasMany(p => p.Transactions)
            .WithOne()
            .HasForeignKey(t => t.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(p => p.Transactions)
            .HasField("_transactions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasIndex(p => p.OrderId).IsUnique()
            .HasDatabaseName("IX_Payments_OrderId");
        builder.HasIndex(p => new { p.TenantId, p.Status })
            .HasDatabaseName("IX_Payments_Tenant_Status");
    }
}

