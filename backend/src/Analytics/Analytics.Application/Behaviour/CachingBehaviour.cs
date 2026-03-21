using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Analytics.Application.Caching;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.Behaviour;

public sealed class CachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
{
    private readonly ICacheService _cache;
    private readonly CacheKeys _cacheKeys;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
    
    public CachingBehavior(ICacheService cache, ILogger<CachingBehavior<TRequest, TResponse>> logger, CacheKeys cacheKeys)
    {
        _cache = cache;
        _logger = logger;
        _cacheKeys = cacheKeys;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var hash = CreateHash(request);
        var cacheKey = _cacheKeys.Resolve($"{request.CacheKeyIdentifier}:{hash}");
        
        var cached = await _cache.GetAsync<TResponse>(cacheKey, ct);
        if (cached is not null)
        {
            _logger.LogInformation("Cache HIT: {Key}", cacheKey);
            return cached;
        }

        _logger.LogInformation("Cache MISS: {Key}", cacheKey);
        var response = await next();
        if (response is not null)
            
            await _cache.SetAsync(cacheKey, response, request.CacheTtl, ct);
        
        return response;
    }

    private string CreateHash(TRequest request)
    {
        var properties = typeof(TRequest)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(p => p.Name != nameof(ICacheableQuery.CacheKeyIdentifier)
                                && p.Name != nameof(ICacheableQuery.CacheTtl))
            .OrderBy(p => p.Name)
            .ToDictionary(p => p.Name, p => p.GetValue(request));
        
        var json = JsonSerializer.Serialize(properties);
        _logger.LogInformation($"[Hash input {typeof(TRequest).Name}]: {json}");

        return Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(json)));
    }
    
}