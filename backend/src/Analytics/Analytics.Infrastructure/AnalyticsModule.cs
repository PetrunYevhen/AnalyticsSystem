using Analytics.Application.Contracts;
using Analytics.Infrastructure.Configuration;
using Autofac;
using MediatR;

namespace Analytics.Infrastructure;

public class AnalyticsModule : IAnalyticsModule
{
    public async Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command)
    {
        return await CommandsExecutor.Execute(command);
    }

    public async Task ExecuteCommandAsync(ICommand command)
    {
        await CommandsExecutor.Execute(command);
    }
    

    public Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
    {
        using (var scope = AnalyticsCompositionRoot.BeginLifetimeScope())
        {
            var mediator = scope.Resolve<IMediator>();
            return mediator.Send(query);
        }
    }
}