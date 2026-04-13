using Analytics.Domain.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Analytics.Infrastructure.EntityTypeConfiguration;

public class TenantEntityTypeConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        
        builder.HasKey(p => p.Id);
        builder.Property(prop => prop.CompanyName).IsRequired();
        builder.Property(prop => prop.Email).IsRequired();
        builder.Property(prop => prop.PasswordHash).IsRequired();
        builder.Property(prop => prop.ApiKey).IsRequired();
        builder.Property(prop => prop.CreatedAt).IsRequired();
        
        builder.HasIndex(prop => prop.Email).IsUnique();
        builder.HasIndex(prop => prop.ApiKey).IsUnique();
    }
}