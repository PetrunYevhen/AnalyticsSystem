namespace Analytics.Application.Queries.Marketing.ExportMarketingExpenses.Dtos;

public class ExportMarketingExpenseDto
{
    public Guid ExpenseId { get; set; }
    public string AdSource { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public int? Impressions { get; set; }
    public int? Clicks { get; set; }
    public int? Leads { get; set; }
    public string? CampaignName { get; set; }
    public string Currency { get; set; } = default!;

}