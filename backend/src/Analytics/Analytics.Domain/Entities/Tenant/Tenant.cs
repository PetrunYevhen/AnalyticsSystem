using Domain;

namespace Analytics.Domain.Entities.Tenant;

public sealed class Tenant : Entity
{
    public string CompanyName { get; private set; } = default!;
    public string ApiKeyHash { get; private set; } = default!;
    public string ApiKeyPrefix { get; private set; } = default!;
    
    private Tenant() { }
    
    public Tenant(string companyName, string apiKeyHash, string apiKeyPrefix)
    {
        Id = Guid.NewGuid();
        CompanyName = companyName;
        ApiKeyHash = apiKeyHash;
        ApiKeyPrefix = apiKeyPrefix;
        CreatedAt = DateTime.UtcNow;
    }

    public static Tenant Create(
        string companyName,
        string apiKeyHash,
        string apiKeyPrefix,
        TimeProvider timeProvider)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Назва компанії не може бути порожньою.", nameof(companyName));

        if (string.IsNullOrWhiteSpace(apiKeyHash))
            throw new ArgumentException("Хеш API ключа не може бути порожнім.", nameof(apiKeyHash));

        return new Tenant
        {
            Id = Guid.NewGuid(),
            CompanyName = companyName.Trim(),
            ApiKeyHash = apiKeyHash,
            ApiKeyPrefix = apiKeyPrefix,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        };
    }
}