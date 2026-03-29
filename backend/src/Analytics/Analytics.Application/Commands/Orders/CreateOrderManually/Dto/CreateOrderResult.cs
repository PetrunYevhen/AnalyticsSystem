namespace Analytics.Application.Commands.Orders.CreateOrderManually.Dto;

public class CreateOrderResult
{
    public CreateOrderResult(Guid orderId, Guid customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
    }

    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
}