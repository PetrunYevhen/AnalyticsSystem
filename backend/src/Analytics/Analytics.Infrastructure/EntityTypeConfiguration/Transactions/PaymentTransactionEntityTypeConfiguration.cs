using Analytics.Domain.Entities.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValueObjects.ValueObject;

namespace Analytics.Infrastructure.EntityTypeConfiguration.Transactions;

public class PaymentTransactionEntityTypeConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions", "Analytics");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.TenantId).IsRequired();
        builder.Property(t => t.PaymentId).IsRequired();
        builder.Property(t => t.Method).IsRequired();
        builder.Property(t => t.Note).HasMaxLength(500);
        builder.Property(t => t.PaidAt).IsRequired();
        builder.Property(p => p.Method).HasConversion<string>().IsRequired();
        builder.Property(p => p.Type).HasConversion<string>().IsRequired();


        builder.ComplexProperty(t => t.Amount, m =>
        {
            m.Property(x => x.Amount).HasColumnName("Amount").HasColumnType("numeric(18,2)");
            m.Property(x => x.Currency).HasColumnName("Currency").HasMaxLength(3)
                .HasConversion(c => c.Code, code => Currency.Parse(code));
        });

        builder.HasIndex(t => t.PaymentId)
            .HasDatabaseName("IX_PaymentTransactions_PaymentId");
    }
}