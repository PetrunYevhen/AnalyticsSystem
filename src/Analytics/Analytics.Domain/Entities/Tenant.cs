using System.Security.Cryptography;

namespace Analytics.Domain.Entities;

public class Tenant
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string ApiKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private Tenant(){}
    
    public Tenant(string name, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Name = name;
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
}