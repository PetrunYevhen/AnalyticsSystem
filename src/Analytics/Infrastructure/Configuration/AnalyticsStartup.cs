using Autofac;

namespace Infrastructure.Configuration;

public class AnalyticsStartup
{
    private static IContainer _container;

    public static void Initialize(
        string connectionString)
    {
        
    }

    private static void ConfigureCompositionRoot(string connectionString)
    {
        var builder = new ContainerBuilder();
        _container = builder.Build();
        AnalyticsCompositionRoot.SetContainer(_container);
    }
}