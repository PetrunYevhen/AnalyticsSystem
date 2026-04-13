namespace Analytics.Application.Queries.MarketingExpenses.Dtos;

public record MarketingExpensesDashdoardDto(
    MarketingStatsDto MarketingStats,
    List<TrafficSourceItemDto> MarketingListItems);