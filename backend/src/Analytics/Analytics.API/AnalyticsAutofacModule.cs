using Analytics.Application.Contracts;
using Analytics.Infrastructure;
using Autofac;

namespace Analytics.API;

public class AnalyticsAutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AnalyticsModule>().
            As<IAnalyticsModule>()
            .SingleInstance();
                
    }
}