using Analytics.Domain.Entities.Tenant;

namespace Analytics.Application.Auth.ApiKey;

public interface IApiKeyService
{
    (Tenant tenant, string plainApiKey) Create(string companyName);
    bool Verify(string plainKey, string hash);
}