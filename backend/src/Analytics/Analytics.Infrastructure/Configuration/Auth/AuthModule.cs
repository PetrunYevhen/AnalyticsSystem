using Analytics.Application.Auth;
using Analytics.Application.Auth.ApiKey;
using Analytics.Infrastructure.Auth;
using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Analytics.Infrastructure.Configuration.Auth;

public class AuthModule : Module
{
    private readonly IConfiguration _configuration;
    public AuthModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(_configuration)
            .As<IConfiguration>()
            .SingleInstance();

        builder.RegisterType<JwtProvider>()
            .As<IJwtProvider>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<HttpContextAccessor>()
            .As<IHttpContextAccessor>()
            .SingleInstance();

        builder.RegisterType<TenantContext>()
            .As<ITenantContext>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<UserContext>()
            .As<IUserContext>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<PasswordHasher>()
            .As<IPasswordHasher>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<ApiKeyService>()
            .As<IApiKeyService>()
            .InstancePerLifetimeScope();
    }
}