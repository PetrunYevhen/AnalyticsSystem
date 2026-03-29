using Analytics.Application.Contracts;
using FluentResults;

namespace Analytics.Application.Commands.Transactions.RefundTransaction;

public class RefundTransactionCommand : CommandBase<Result>
{
    public RefundTransactionCommand(Guid orderId, decimal amount, string? note, string method)
    {
        OrderId = orderId;
        Amount = amount;
        Note = note;
        Method = method;
    }

    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; }
    public string? Note { get; set; }
}