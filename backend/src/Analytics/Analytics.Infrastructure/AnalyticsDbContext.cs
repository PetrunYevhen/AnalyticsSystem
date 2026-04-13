using Analytics.Domain.Entities;
using Analytics.Domain.Entities.Customer;
using Analytics.Domain.Entities.Order;
using Analytics.Domain.Entities.Source;
using Analytics.Domain.Entities.Tenant;
using Analytics.Infrastructure.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure;

public class AnalyticsDbContext : DbContext
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<MarketingExpense> MarketingExpenses { get; set; }
    public DbSet<DataSource> DataSources { get; set; }

    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Analytics");
        
        modelBuilder.ApplyConfiguration(new TenantEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MarketingExpenseEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new DataSourceEntityTypeConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}