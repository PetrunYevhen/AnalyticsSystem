using Analytics.Application.Contracts;
using FluentResults;

namespace Analytics.Application.Commands.Transactions.CancelTransaction;

public class CancelTransactionCommand  : CommandBase<Result>
{
    public Guid OrderId { get; private set; }
    public string? Note { get; private set; }
    
    public CancelTransactionCommand(Guid orderId,string? note)
    {
        OrderId = orderId;
        Note = note;
    }
}