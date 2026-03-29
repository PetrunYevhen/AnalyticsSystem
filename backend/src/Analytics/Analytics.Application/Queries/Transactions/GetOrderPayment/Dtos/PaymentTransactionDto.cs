namespace Analytics.Application.Queries.Transactions.GetOrderPayment.Dtos;

public class PaymentTransactionDto
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public required string Method { get; init; }
    public DateTime PaidAt { get; init; }
    public required string Type { get; init; }
    public string? Note { get; init; }
}