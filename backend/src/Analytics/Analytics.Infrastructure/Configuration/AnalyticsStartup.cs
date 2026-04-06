using Analytics.Infrastructure.Configuration.Data;
using Analytics.Infrastructure.Configuration.Mediation;
using Autofac;

namespace Analytics.Infrastructure.Configuration;

public class AnalyticsStartup
{
    private static IContainer _container;

    public static void Initialize(
        string connectionString)
    {
        ConfigureCompositionRoot(connectionString);
    }

    private static void ConfigureCompositionRoot(string connectionString)
    {
        var builder = new ContainerBuilder();
        
        builder.RegisterModule(new DataAccessModule(connectionString));
        builder.RegisterModule(new MediatorModule());
        
        _container = builder.Build();
        AnalyticsCompositionRoot.SetContainer(_container);
    }
}