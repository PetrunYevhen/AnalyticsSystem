using Analytics.Application.Contracts;
using Autofac;

namespace Analytics.Infrastructure.Configuration.Metrics;

public class MetricsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var applicationAssembly = typeof(ApplicationAssembly).Assembly;

        builder.RegisterAssemblyTypes(applicationAssembly)
            .Where(t => t.Name.EndsWith("StatsProvider")
                        || t.Name.EndsWith("Calculator")
                        || t.Name.EndsWith("Reader"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}