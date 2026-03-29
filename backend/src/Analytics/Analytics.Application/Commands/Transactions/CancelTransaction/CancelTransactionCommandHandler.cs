using Analytics.Application.Auth;
using Analytics.Domain.RepositoryContracts;
using FluentResults;
using MediatR;

namespace Analytics.Application.Commands.Transactions.CancelTransaction;

public class CancelTransactionCommandHandler : IRequestHandler<CancelTransactionCommand, Result>
{
    private readonly ITenantContext _tenantContext;
    private readonly IPaymentRepository _paymentRepository;
    
    public CancelTransactionCommandHandler(ITenantContext tenantContext, IPaymentRepository paymentRepository)
    {
        _tenantContext = tenantContext;
        _paymentRepository = paymentRepository;
    }

    public async Task<Result> Handle(CancelTransactionCommand request, CancellationToken cancellationToken)
    {
        
        var tenantId = _tenantContext.RequiredTenantId();
        
        var payment = await _paymentRepository.GetByOrderIdWithTransactionsAsync(tenantId, request.OrderId, cancellationToken);
        if (payment is null)
            return Result.Fail($"Оплата {request.OrderId} не знайдена");
        
        var cancelResult = payment.Cancel(DateTime.Now, request.Note);
        if (cancelResult.IsFailed)
            return cancelResult.ToResult();

        await _paymentRepository.AddTransactionAsync(cancelResult.Value, cancellationToken);

        return Result.Ok();
    }
}