using Analytics.Application.Contracts;
using FluentResults;

namespace Analytics.Application.Commands.Transactions.RecordTransaction;

public class RecordTransactionCommand : CommandBase<Result>
{
    public RecordTransactionCommand(Guid orderId, decimal amount, string method, string? note)
    {
        OrderId = orderId;
        Amount = amount;
        Method = method;
        Note = note;
    }

    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; }
    public string? Note { get; set; }
}