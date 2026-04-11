using System.Text.Json;
using Analytics.Application.Caching;
using Analytics.Infrastructure.Caching.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Analytics.Infrastructure.Caching;

public sealed class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCacheService> _logger;

    private static readonly JsonSerializerOptions _options = new()
    {
        Converters = { new ResultJsonConverterFactory() }
    };

    
    public RedisCacheService(
        IConnectionMultiplexer redis,
        ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);
            if (!value.HasValue) return default;
            return JsonSerializer.Deserialize<T>(value!,  _options);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis get failed for key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value,  _options);
            await db.StringSetAsync(key, bytes, ttl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis set failed for key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis remove failed for key {Key}", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{prefix}*").ToArray();
            if (keys.Length == 0) return;
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(keys);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis remove by prefix failed for prefix {Prefix}", prefix);
        }
    }
}