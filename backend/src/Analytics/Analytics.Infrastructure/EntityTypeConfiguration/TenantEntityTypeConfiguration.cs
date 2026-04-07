using Analytics.Domain.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Analytics.Infrastructure.EntityTypeConfiguration;

public class TenantEntityTypeConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants", "Analytics");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.ApiKeyHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(t => t.ApiKeyPrefix)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(t => t.CreatedAt).IsRequired();

        builder.HasIndex(t => t.ApiKeyPrefix)
            .IsUnique()
            .HasDatabaseName("IX_Tenants_ApiKeyPrefix");

        builder.HasIndex(t => t.ApiKeyHash)
            .IsUnique()
            .HasDatabaseName("IX_Tenants_ApiKeyHash");

        builder.HasIndex(t => t.CompanyName)
            .HasDatabaseName("IX_Tenants_CompanyName");
    }
}