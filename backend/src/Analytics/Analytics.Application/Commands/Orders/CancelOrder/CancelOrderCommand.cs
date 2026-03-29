using Analytics.Application.Caching;
using Analytics.Application.Contracts;
using FluentResults;

namespace Analytics.Application.Commands.Orders.CancelOrder;

public class CancelOrderCommand : CommandBase<Result>
{
    public CancelOrderCommand(Guid orderId, string? note)
    {
        OrderId = orderId;
        Note = note;
    }

    public Guid OrderId { get; set; }
    public string? Note { get; set; }
}