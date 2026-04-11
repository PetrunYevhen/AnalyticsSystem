using Analytics.Infrastructure.Dapper;
using Application;
using Autofac;
using Dapper;
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
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        
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
            .InstancePerLifetimeScope();
        
        builder.Register(c => c.Resolve<AnalyticsDbContext>())
            .As<DbContext>()
            .InstancePerLifetimeScope();
        
        var infrastructureAssembly = typeof(AnalyticsDbContext).Assembly;
        
        builder.RegisterAssemblyTypes(infrastructureAssembly)
            .Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("StatsReader"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}