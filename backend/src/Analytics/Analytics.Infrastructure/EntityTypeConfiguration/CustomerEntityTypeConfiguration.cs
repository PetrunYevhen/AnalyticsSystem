using Analytics.Domain.Entities.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Analytics.Infrastructure.EntityTypeConfiguration;

public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        
        builder.HasKey(customer => customer.Id);
        builder.Property(prop => prop.FullName).IsRequired();
        builder.Property(prop => prop.ExternalId).IsRequired();
        builder.Property(prop => prop.CreatedAt).IsRequired();
        builder.Property(prop => prop.RegistrationDate).IsRequired();
        builder.Property(prop => prop.CreatedAt).IsRequired();
        
        builder.Property(prop => prop.Email).IsRequired(false).HasMaxLength(255);
        builder.Property(prop => prop.PhoneNumber).IsRequired(false).HasMaxLength(50);
        
        builder.Property(prop => prop.Status). HasConversion<string>();
        
        builder.HasIndex(customer => new { customer.TenantId, customer.ExternalId }).IsUnique();
    }
}