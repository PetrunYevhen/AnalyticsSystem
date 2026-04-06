using Analytics.Domain.Entities;
using Analytics.Infrastructure.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure;

public class AnalyticsDbContext : DbContext
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<MarketingExpense> MarketingExpenses { get; set; }

    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Analytics");
        
        modelBuilder.ApplyConfiguration(new TenantEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}