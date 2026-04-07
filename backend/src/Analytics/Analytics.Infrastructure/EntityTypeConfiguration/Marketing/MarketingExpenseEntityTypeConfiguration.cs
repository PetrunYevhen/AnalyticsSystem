using Analytics.Domain.Entities.Marketing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValueObjects.ValueObject;

namespace Analytics.Infrastructure.EntityTypeConfiguration.Marketing;

public class MarketingExpenseEntityTypeConfiguration : IEntityTypeConfiguration<MarketingExpense>
{
    public void Configure(EntityTypeBuilder<MarketingExpense> builder)
    {
        builder.ToTable("MarketingExpenses", "Analytics");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.TenantId).IsRequired();

        builder.Property(m => m.CampaignId);

        builder.ComplexProperty(m => m.Amount, money =>
        {
            money.Property(p => p.Amount)
                .HasColumnName("Amount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            money.Property(p => p.Currency)
                .HasColumnName("Currency")
                .HasConversion(
                    currency => currency.Code,
                    code => new Currency(code)
                    )
                .HasMaxLength(3) 
                .IsRequired();
        });

        builder.Property(m => m.AdSource)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.ExpenseDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(m => m.CreatedAt).IsRequired();
        builder.Property(m => m.MetricsRecordedAt);

        builder.Property(m => m.Impressions).IsRequired();
        builder.Property(m => m.Clicks).IsRequired();
        builder.Property(m => m.Leads).IsRequired();

        builder.Ignore(m => m.ClickThroughRate);
        builder.Ignore(m => m.ConversionRate);

        builder.HasIndex(m => new { m.TenantId, m.ExpenseDate })
            .HasDatabaseName("IX_MarketingExpenses_Tenant_Date");

        builder.HasIndex(m => new { m.TenantId, m.AdSource, m.ExpenseDate })
            .HasDatabaseName("IX_MarketingExpenses_Tenant_Source_Date");

        builder.HasIndex(m => new { m.TenantId, m.CampaignId })
            .HasDatabaseName("IX_MarketingExpenses_Tenant_Campaign");
    }
}