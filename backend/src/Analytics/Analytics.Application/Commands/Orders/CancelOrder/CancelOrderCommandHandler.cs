using Analytics.Application.Auth;
using Analytics.Domain.RepositoryContracts;
using FluentResults;
using MediatR;

namespace Analytics.Application.Commands.Orders.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly ITenantContext _tenantContext;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(
        ITenantContext tenantContext,
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository)
    {
        _tenantContext = tenantContext;
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
    }

    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();

        var order = await _orderRepository.GetByIdAsync(tenantId, request.OrderId, cancellationToken);
        if (order is null)
            return Result.Fail("Замовлення не знайдено.");

        var payment = await _paymentRepository.GetByOrderIdWithTransactionsAsync(tenantId, request.OrderId, cancellationToken);

        if (payment is not null)
        {
            var cancelResult = payment.Cancel(DateTime.UtcNow, request.Note);
            if (cancelResult.IsFailed)
                return cancelResult.ToResult();

            await _paymentRepository.AddTransactionAsync(cancelResult.Value, cancellationToken);
        }

        order.MarkAsCanceled();
        await _orderRepository.UpdateAsync(order, cancellationToken);

        return Result.Ok();
    }
}