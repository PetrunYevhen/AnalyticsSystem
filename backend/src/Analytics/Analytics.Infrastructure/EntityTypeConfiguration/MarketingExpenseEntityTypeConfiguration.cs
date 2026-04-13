using Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Analytics.Infrastructure.EntityTypeConfiguration;

public class MarketingExpenseEntityTypeConfiguration : IEntityTypeConfiguration<MarketingExpense>
{
    public void Configure(EntityTypeBuilder<MarketingExpense> builder)
    {

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Amount)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(m => m.Currency)
            .IsRequired()
            .HasMaxLength(3); 

        builder.Property(m => m.AdSource)
            .IsRequired()
            .HasMaxLength(100); 

        builder.Property(m => m.CampaignName)
            .HasMaxLength(255);

        builder.Property(m => m.Date)
            .IsRequired();
        
        builder.HasIndex(m => new { m.TenantId, m.Date });
    }
}