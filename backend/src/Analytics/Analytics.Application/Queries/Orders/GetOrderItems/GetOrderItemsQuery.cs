using Analytics.Application.Contracts;
using Analytics.Application.Queries.Orders.GetOrderItems.Dtos;
using FluentResults;

namespace Analytics.Application.Queries.Orders.GetOrderItems;

public class GetOrderItemsQuery : QueryBase<Result<List<OrderItemDto>>>
{
    public GetOrderItemsQuery(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; set; }
}