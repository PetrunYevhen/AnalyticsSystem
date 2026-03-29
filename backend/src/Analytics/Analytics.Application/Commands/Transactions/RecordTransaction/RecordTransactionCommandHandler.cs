using Analytics.Application.Auth;
using Analytics.Domain.Entities.Transactions;
using Analytics.Domain.Entities.Transactions.Enums;
using Analytics.Domain.RepositoryContracts;
using FluentResults;
using MediatR;

namespace Analytics.Application.Commands.Transactions.RecordTransaction;

public class RecordTransactionCommandHandler : IRequestHandler<RecordTransactionCommand, Result>
{
    private readonly ITenantContext _tenantContext;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;

    public RecordTransactionCommandHandler(ITenantContext tenantContext, IPaymentRepository paymentRepository, IOrderRepository orderRepository)
    {
        _tenantContext = tenantContext;
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
    }

    public async Task<Result> Handle(RecordTransactionCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            return Result.Fail("Сума має бути більше нуля.");

        if (!Enum.TryParse<PaymentMethod>(request.Method, ignoreCase: true, out var method))
            return Result.Fail($"Невідомий метод оплати: '{request.Method}'");

        var tenantId = _tenantContext.RequiredTenantId();
        var order = await _orderRepository.GetByIdAsync(tenantId, request.OrderId, cancellationToken);

        var payment = await _paymentRepository.GetByOrderIdAsync(tenantId, request.OrderId, cancellationToken);
        if (payment is null)
        {
            payment = Payment.Create(tenantId, request.OrderId, order.TotalAmount, DateTime.UtcNow);
            await _paymentRepository.AddAsync(payment, cancellationToken);
        }
        
        
        var result = payment.RecordTransaction(request.Amount, method, request.Note, DateTime.UtcNow);
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        await _paymentRepository.AddTransactionAsync(result.Value, cancellationToken);
        if (payment.Status == PaymentStatus.Paid)
            order.MarkAsPaid();
        

        return Result.Ok();
    }
}