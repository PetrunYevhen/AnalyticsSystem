using Analytics.Application.Auth;
using Analytics.Application.Commands.Orders.CreateOrderManually.Dto;
using Analytics.Domain.Entities.Customer;
using Analytics.Domain.Entities.Order;
using Analytics.Domain.Entities.Transactions;
using Analytics.Domain.Entities.Transactions.Enums;
using Analytics.Domain.RepositoryContracts;
using FluentResults;
using MediatR;
using ValueObjects.ValueObject;

namespace Analytics.Application.Commands.Orders.CreateOrderManually;

public class CreateOrderManuallyCommandHandler : IRequestHandler<CreateOrderManuallyCommand,  Result<CreateOrderResult>>
{
    private readonly ITenantContext _tenantContext;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPaymentRepository _paymentRepository;

    public CreateOrderManuallyCommandHandler(ITenantContext tenantContext, IOrderRepository requestRepository, ICustomerRepository customerRepository, IPaymentRepository paymentRepository)
    {
        _tenantContext = tenantContext;
        _orderRepository = requestRepository;
        _customerRepository = customerRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<CreateOrderResult>> Handle(CreateOrderManuallyCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        var currency = Currency.Parse(request.Order.Currency);

        if (!Enum.TryParse<OrderStatus>(request.Order.Status, ignoreCase: true, out var status))
            return Result.Fail($"Unknown order status: '{request.Order.Status}'");


        var customer = await _customerRepository.GetByEmailAsync(tenantId, request.Customer.Email, cancellationToken);
        var isNewCustomer = customer is null;

        if (isNewCustomer)
        {
            if (string.IsNullOrWhiteSpace(request.Customer.FullName))
                return Result.Fail("FullName обов'язковий для нового клієнта.");
            
            customer = Customer.Create(
                tenantId,
                request.Customer.ExternalId,
                request.Customer.FullName,
                request.Customer.PhoneNumber,
                request.Customer.Email,
                DateTime.UtcNow,
                request.Customer.AcquisitionChannel,
                request.Customer.CampaignId
            );
        }

        if (!string.IsNullOrWhiteSpace(request.Order.ExternalOrderId))
        {
            var exists = await _orderRepository.ExistsByExternalIdAsync(
                tenantId,
                request.Order.ExternalOrderId, cancellationToken);
            if (exists)
                return Result.Fail<CreateOrderResult>(
                    "Order with this ExternalOrderId already exists");
        }

        

        var newOrder = Order.Create(
            tenantId,
            customer.Id,
            request.Order.ExternalOrderId,
            currency,
            request.Order.OrderDate,
            status
        );
        foreach (var i in request.Order.Items)
        {
            newOrder.AddItem(
                i.ProductExternalId,
                i.ProductName,
                i.Category,
                i.Quantity,
                i.UnitPrice,
                i.UnitCost
            );
        }

        customer.AddOrder(newOrder, DateTime.UtcNow);
        var payment = Payment.Create(
                tenantId,
                newOrder.Id,
                newOrder.TotalAmount,
                DateTime.UtcNow);


        if (request.Order.InitialPaymentAmount is > 0)
        {
            if (string.IsNullOrWhiteSpace(request.Order.InitialPaymentMethod))
                return Result.Fail("Вкажіть метод оплати.");

            if (!Enum.TryParse<PaymentMethod>(request.Order.InitialPaymentMethod, ignoreCase: true,
                    out var paymentMethod))
                return Result.Fail($"Невідомий метод оплати: '{request.Order.InitialPaymentMethod}'");

            var transactionResult = payment.RecordTransaction(
                request.Order.InitialPaymentAmount.Value,
                paymentMethod,
                request.Order.InitialPaymentNote,
                DateTime.UtcNow);

            if (transactionResult.IsFailed)
                return Result.Fail(transactionResult.Errors);

            await _paymentRepository.AddTransactionAsync(transactionResult.Value, cancellationToken);
        }

        if (isNewCustomer)
            await _customerRepository.AddAsync(customer, cancellationToken);

        await _orderRepository.AddAsync(newOrder, cancellationToken);
        await _paymentRepository.AddAsync(payment, cancellationToken);

        return Result.Ok(new CreateOrderResult(newOrder.Id, customer.Id));
    }
}