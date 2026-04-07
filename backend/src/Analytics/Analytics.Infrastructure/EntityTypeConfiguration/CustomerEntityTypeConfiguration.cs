using Analytics.Domain.Entities.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Analytics.Infrastructure.EntityTypeConfiguration;

public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{

    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", "Analytics");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ExternalId)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .HasMaxLength(255);

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(c => c.CampaignId);

        builder.Property(c => c.AcquisitionChannel)
            .HasConversion<string>()
            .HasMaxLength(128)
            .IsRequired(false);
        
        builder.Property(c => c.RegistrationDate).IsRequired();
        builder.Property(c => c.FirstOrderDate);
        builder.Property(c => c.LastOrderDate);
        builder.Property(c => c.OrderCount).IsRequired();
        
        builder.Property(c => c.TotalRevenue).IsRequired();
        
        builder.Property(c => c.CreatedAt).IsRequired();

        builder.Property(c => c.TenantId).IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(c => new { c.TenantId, c.TotalRevenue })
            .HasDatabaseName("IX_Customers_Tenant_TotalRevenue");
        
        builder.HasIndex(c => new { c.TenantId, c.ExternalId })
            .IsUnique()
            .HasDatabaseName("IX_Customers_Tenant_External");

        builder.HasIndex(c => new { c.TenantId, c.RegistrationDate })
            .HasDatabaseName("IX_Customers_Tenant_RegistrationDate");

        builder.HasIndex(c => new { c.TenantId, c.LastOrderDate })
            .HasDatabaseName("IX_Customers_Tenant_LastOrderDate");

        builder.HasIndex(c => new { c.TenantId, c.Status })
            .HasDatabaseName("IX_Customers_Tenant_Status");

        builder.HasIndex(c => new { c.TenantId, c.AcquisitionChannel })
            .HasDatabaseName("IX_Customers_Tenant_Channel");
        
        builder.HasIndex(c => new { c.TenantId, c.Email })
            .IsUnique()
            .HasDatabaseName("IX_Customers_Tenant_Email");
    }
}