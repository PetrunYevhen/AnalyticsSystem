using Analytics.Application.Caching;
using Analytics.Infrastructure.Caching;
using Autofac;
using StackExchange.Redis;

namespace Analytics.Infrastructure.Configuration.Cache;

public class CachingModule : Module
{
    private readonly string _connectionString;

    public CachingModule(string connectionString)
    {
        _connectionString = connectionString;
    }


    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<RedisCacheService>()
            .As<ICacheService>()
            .SingleInstance();
        
        builder.RegisterType<CacheKeys>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.Register(_ =>
        {
            var config = ConfigurationOptions.Parse(_connectionString);
            config.AbortOnConnectFail = false;
            config.ConnectRetry = 3;
            config.ConnectTimeout = 5000;

            return ConnectionMultiplexer.Connect(config);            
                  
        })
        .As<IConnectionMultiplexer>()
        .SingleInstance();;
    }
}