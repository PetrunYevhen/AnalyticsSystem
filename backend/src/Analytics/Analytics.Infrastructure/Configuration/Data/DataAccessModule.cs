using System.Data;
using System.Data.Common;
using Application;
using Autofac;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Configuration.Data;

public class DataAccessModule : Module
{
    private readonly string _connectionString;

    public DataAccessModule(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<NpgsqlConnectionFactory>()
            .As<INpgsqlConnectionFactory>()
            .WithParameter("connectionString", _connectionString)
            .InstancePerLifetimeScope();
        
        builder.Register(c =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<AnalyticsDbContext>();
                optionsBuilder.UseNpgsql(_connectionString);
                return new AnalyticsDbContext(optionsBuilder.Options);
            })
            .AsSelf()
            .As<DbContext>()
            .InstancePerLifetimeScope();
        
        var infrastructureAssembly = typeof(AnalyticsDbContext).Assembly;
        
        builder.RegisterAssemblyTypes(infrastructureAssembly)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .FindConstructorsWith(new AllConstructorFinder());
    }
}