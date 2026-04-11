using System.Security.Cryptography;
using System.Text;
using Analytics.Application.Auth.ApiKey;
using Analytics.Domain.Entities.Tenant;
using Microsoft.Extensions.Configuration;

namespace Analytics.Infrastructure.Auth;

public class ApiKeyService : IApiKeyService
{
    private readonly byte[] _pepper;

    public ApiKeyService(IConfiguration configuration)
    {
        var pepper = configuration["ApiKeySettings:Pepper"];
        if (string.IsNullOrWhiteSpace(pepper))
            throw new InvalidOperationException("ApiKeySettings:Pepper не знайдено в конфігурації.");
        _pepper = Encoding.UTF8.GetBytes(pepper);
    }

    public (Tenant tenant, string plainApiKey) Create(string companyName)
    {
        var plainKey = GeneratePlainApiKey();
        var hash = ComputeHash(plainKey);
        var prefix = plainKey[..15];
        var tenant = new Tenant(companyName, hash, prefix);
        return (tenant, plainKey);
    }

    public bool Verify(string plainKey, string hash)
    {
        if (string.IsNullOrWhiteSpace(plainKey) || string.IsNullOrWhiteSpace(hash))
            return false;
        try
        {
            var computed = ComputeHash(plainKey);
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(computed),
                Convert.FromHexString(hash)
            );
        }
        catch { return false; }
    }

    private string ComputeHash(string plainKey) =>
        Convert.ToHexString(HMACSHA256.HashData(_pepper, Encoding.UTF8.GetBytes(plainKey)));

    private static string GeneratePlainApiKey()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return $"sk_live_{Convert.ToHexString(bytes).ToLowerInvariant()}";
    }
}