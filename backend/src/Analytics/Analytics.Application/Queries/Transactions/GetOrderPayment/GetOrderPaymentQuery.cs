using Analytics.Application.Contracts;
using Analytics.Application.Queries.Transactions.GetOrderPayment.Dtos;
using FluentResults;

namespace Analytics.Application.Queries.Transactions.GetOrderPayment;

public class GetOrderPaymentQuery : QueryBase<Result<OrderPaymentDto>>
{
    public GetOrderPaymentQuery(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; init; }
}