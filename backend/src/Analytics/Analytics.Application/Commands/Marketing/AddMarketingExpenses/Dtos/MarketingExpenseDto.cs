namespace Analytics.Application.Commands.Marketing.AddMarketingExpenses.Dtos;

public record MarketingExpenseDto(
    Guid     Id,
    string   AdSource,
    DateOnly ExpenseDate,
    decimal  Amount,
    string   Currency,
    Guid?    CampaignId,
    int?     Impressions,
    int?     Clicks,
    int?     Leads,
    double?  ClickThroughRate,
    double?  ConversionRate
);