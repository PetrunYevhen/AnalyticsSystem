using Analytics.Domain.Entities.Marketing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValueObjects.ValueObject;

namespace Analytics.Infrastructure.EntityTypeConfiguration.Marketing;

public class CampaignEntityTypeConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("Campaigns", "Analytics");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.TenantId).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Channel)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.ComplexProperty(c => c.Budget, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("BudgetAmount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("BudgetCurrency")
                .HasConversion(
                    currency => currency.Code,
                    code => new Currency(code))
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.ComplexProperty(c => c.ActivePeriod, period =>
        {
            period.Property(p => p.StartDate)
                .HasColumnName("ActivePeriodStart")
                .HasColumnType("date")
                .IsRequired();

            period.Property(p => p.EndDate)
                .HasColumnName("ActivePeriodEnd")
                .HasColumnType("date")
                .IsRequired();
        });

        builder.ComplexProperty(p => p.ActualSpend, spend =>
        {
            spend.Property(m => m.Amount)
                .HasColumnName("ActualSpendAmount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            spend.Property(m => m.Currency)
                .HasColumnName("ActualSpendCurrency")
                .HasConversion(
                    currency => currency.Code,
                    code => new Currency(code))
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasIndex(c => new { c.TenantId, c.Name })
            .IsUnique()
            .HasDatabaseName("IX_Campaigns_Tenant_Name");

        builder.HasIndex(c => new { c.TenantId, c.Status })
            .HasDatabaseName("IX_Campaigns_Tenant_Status");

        builder.HasIndex(c => new { c.TenantId, c.Channel })
            .HasDatabaseName("IX_Campaigns_Tenant_Channel");
    }
}