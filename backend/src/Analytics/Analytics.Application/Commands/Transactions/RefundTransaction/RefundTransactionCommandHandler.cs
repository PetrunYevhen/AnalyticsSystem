using Analytics.Application.Auth;
using Analytics.Domain.Entities.Transactions.Enums;
using Analytics.Domain.RepositoryContracts;
using FluentResults;
using MediatR;

namespace Analytics.Application.Commands.Transactions.RefundTransaction;

public class RefundTransactionCommandHandler : IRequestHandler<RefundTransactionCommand, Result>
{
    private readonly ITenantContext _tenantContext;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;


    public RefundTransactionCommandHandler(ITenantContext tenantContext, IPaymentRepository paymentRepository, IOrderRepository orderRepository)
    {
        _tenantContext = tenantContext;
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
    }

    public async Task<Result> Handle(RefundTransactionCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<PaymentMethod>(request.Method, ignoreCase: true, out var method)) 
            return Result.Fail($"Невідомий метод оплати: '{request.Method}'");
        var tenantId = _tenantContext.RequiredTenantId();
        
        var payment = await _paymentRepository.GetByOrderIdWithTransactionsAsync(tenantId, request.OrderId, cancellationToken);
        if (payment is null)
            return Result.Fail("Платіж не знайдено.");

        var result = payment.Refund(DateTime.UtcNow, request.Amount, method, request.Note);
        if (result.IsFailed)
            return Result.Fail(result.Errors);
        var order = await _orderRepository.GetByIdAsync(tenantId, request.OrderId, cancellationToken);
        if (order is null)
            return Result.Fail("Замовлення не знайдено");
        
        await _paymentRepository.AddTransactionAsync(result.Value,  cancellationToken);
        
        return Result.Ok();
    }
}