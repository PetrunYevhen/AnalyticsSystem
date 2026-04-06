using Analytics.Application.Contracts;
using Autofac;
using MediatR;

namespace Analytics.Infrastructure.Configuration;

public class CommandsExecutor
{
    internal static async Task Execute(ICommand command)
    {
        using (var scope = AnalyticsCompositionRoot.BeginLifetimeScope())
        {
            var mediator = scope.Resolve<IMediator>();
            await mediator.Send(command);
        }
    }

    internal static async Task<TResult> Execute<TResult>(ICommand<TResult> command)
    {
        using (var scope = AnalyticsCompositionRoot.BeginLifetimeScope())
        {
            var mediator = scope.Resolve<IMediator>();
            return await mediator.Send(command);
        }
    }
}