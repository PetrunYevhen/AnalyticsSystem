using Analytics.Application.Commands.Marketing.AddMarketingExpenses.Dtos;
using Analytics.Application.Contracts;
using FluentResults;

namespace Analytics.Application.Commands.Marketing.AddMarketingExpenses;

public class AddMarketingExpensesCommand : CommandBase<Result<MarketingExpenseDto>>
{
    public string AdSource { get; init; } = default!;
    public Guid? CampaignId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = default!;
    public DateOnly ExpenseDate { get; init; }

    public int? Impressions { get; init; }
    public int? Clicks { get; init; }
    public int? Leads { get; init; }
}