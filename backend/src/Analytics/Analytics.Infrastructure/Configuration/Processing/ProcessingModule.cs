using Analytics.Application.Behaviour;
using Autofac;
using Infrastructure;
using MediatR;

namespace Analytics.Infrastructure.Configuration.Processing;

public class ProcessingModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(Assemblies.Application)
            .AsClosedTypesOf(typeof(IRequestHandler<>))
            .AsClosedTypesOf(typeof(IRequestHandler<,>))
            .InstancePerLifetimeScope();
        
        builder.RegisterType<UnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();
        
        builder.RegisterGeneric(typeof(TransactionBehavior<,>))
            .As(typeof(IPipelineBehavior<,>))
            .InstancePerLifetimeScope();
        
        builder.RegisterGeneric(typeof(ValidationBehavior<,>))
            .As(typeof(IPipelineBehavior<,>))
            .InstancePerLifetimeScope();
        
        builder.RegisterGeneric(typeof(CachingBehavior<,>))
            .As(typeof(IPipelineBehavior<,>))
            .InstancePerLifetimeScope();
    }
}