using System.Security.Cryptography;
using Analytics.Domain.Entities.Source;

namespace Analytics.Domain.Entities;

public class Tenant
{
    public Guid Id { get; private set; }
    public string AdminFullName { get; private set; }
    public string CompanyName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public SubscriptionPlan SubscriptionPlan { get; private set; }
    public string ApiKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<DataSource> _dataSources = new();
    public IReadOnlyCollection<DataSource> DataSources => _dataSources.AsReadOnly();
    
    private Tenant(){}
    
    public Tenant(string adminFullName, string companyName, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        AdminFullName = adminFullName;
        CompanyName = companyName;
        Email = email;
        PasswordHash = passwordHash;
        ApiKey = GenerateApiKey();
        CreatedAt = DateTime.UtcNow;
    }
    
    private string GenerateApiKey()
    {
        byte[] data = RandomNumberGenerator.GetBytes(32);

        return "analytics_" + Convert.ToBase64String(data)
            .Replace("/", "")
            .Replace("+", "");
    }
    
    public void AddDataSource(DataSourceType type, string name, string config)
    {
        if (_dataSources.Count >= 5) 
            throw new Exception("Limit of data sources reached.");

        _dataSources.Add(new DataSource(this.Id, type, name, config));
    }
}