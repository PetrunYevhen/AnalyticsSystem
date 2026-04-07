using Analytics.Domain.Entities.Customer;
using Analytics.Domain.Entities.Marketing;
using Analytics.Domain.Entities.Order;
using Analytics.Domain.Entities.Tenant;
using Analytics.Domain.Entities.Transactions;
using Analytics.Domain.Entities.User;
using Analytics.Infrastructure.EntityTypeConfiguration;
using Analytics.Infrastructure.EntityTypeConfiguration.Marketing;
using Analytics.Infrastructure.EntityTypeConfiguration.Orders;
using Analytics.Infrastructure.EntityTypeConfiguration.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure;

public class AnalyticsDbContext : DbContext
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<MarketingExpense> MarketingExpenses { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Analytics");
        
        modelBuilder.ApplyConfiguration(new TenantEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MarketingExpenseEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CampaignEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentTransactionEntityTypeConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}