using Autofac;

namespace Analytics.Infrastructure.Configuration;

public class AnalyticsCompositionRoot
{
    private static IContainer _container;

    public static void SetContainer(IContainer container)
    {
        _container = container;
    }

    public static ILifetimeScope BeginLifetimeScope()
    {
        if(_container == null) 
            throw new InvalidOperationException("Container is not initialized");
        
        return _container.BeginLifetimeScope();
    }
}