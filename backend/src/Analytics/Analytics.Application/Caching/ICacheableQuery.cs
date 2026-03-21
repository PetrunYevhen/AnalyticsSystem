namespace Analytics.Application.Caching;

public interface ICacheableQuery
{
    string CacheKeyIdentifier { get; }
    TimeSpan CacheTtl { get; }
}