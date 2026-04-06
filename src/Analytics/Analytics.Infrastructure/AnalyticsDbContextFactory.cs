using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Analytics.Infrastructure;

public class AnalyticsDbContextFactory : IDesignTimeDbContextFactory<AnalyticsDbContext>
{
    public AnalyticsDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        var optionsBuilder = new DbContextOptionsBuilder<AnalyticsDbContext>();
        
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("AnalyticsConnectionString"));
        
        return new AnalyticsDbContext(optionsBuilder.Options);
    }
}