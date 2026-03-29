using CsvHelper.Configuration.Attributes;

namespace Analytics.Application.Commands.Orders.ImportOrders.Dtos;

public class ImportOrdersCsvRow
{
    [Name("OrderId")]
    public string? OrderId { get; set; } 

    [Name("ExternalOrderId")]
    public string ExternalOrderId { get; set; } = default!;

    [Name("CustomerName")]
    public string CustomerFullName { get; set; } = default!;

    [Name("CustomerEmail")]
    public string CustomerEmail { get; set; } = default!;

    [Name("OrderDate")]
    public DateTime OrderDate { get; set; }

    [Name("Status")]
    public string Status { get; set; } = default!;

    [Name("TotalAmount")]
    public decimal TotalAmount { get; set; }
    [Name("Currency")]
    public string Currency { get; set; } = "UAH";
}