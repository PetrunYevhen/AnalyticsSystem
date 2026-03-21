using Analytics.Application.Contracts;
using FluentResults;
using Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.Behaviour;

public class TransactionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not CommandBase<TResponse>)
            return await next();

        var commandName = typeof(TRequest).Name;

        _logger.LogInformation("[Command] Executing {Command}", commandName);

        var response = await next();

        if (response is IResultBase result && result.IsFailed)
        {
            _logger.LogWarning("[Command] {Command} failed: {Errors}",
                commandName, string.Join(", ", result.Errors.Select(e => e.Message)));
            return response;
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("[Command] {Command} committed successfully", commandName);

        return response;
    }
}