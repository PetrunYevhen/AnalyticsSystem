namespace Analytics.Application.Queries.Dashboard.Dtos;

public class RecentTransactionsDto
{
    public Guid Id { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required string Status {get; set;}
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
}