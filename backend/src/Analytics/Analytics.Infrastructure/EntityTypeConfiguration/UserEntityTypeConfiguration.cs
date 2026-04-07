using Analytics.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Analytics.Infrastructure.EntityTypeConfiguration;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(prop=> prop.TenantId).IsRequired();
        builder.Property(prop=> prop.FullName).IsRequired();
        builder.Property(prop=> prop.Email).IsRequired();
        builder.Property(prop=> prop.PasswordHash).IsRequired();
        builder.Property(prop => prop.Role)
            .HasConversion(
                role => role.Name,
                name => UserRole.FromName(name))
            .IsRequired();
        
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.TenantId);
        

    }
}