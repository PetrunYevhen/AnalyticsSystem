namespace Analytics.Application.Queries.Transactions.GetOrderPayment.Dtos;

public class OrderPaymentDto
{
    public Guid PaymentId { get; init; }
    public Guid OrderId { get; init; }
    public required string Status { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal RemainingAmount { get; init; }  
    public decimal RefundedAmount { get; init; }
    public required string Currency { get; init; }
    public required List<PaymentTransactionDto> Transactions { get; init; }
}