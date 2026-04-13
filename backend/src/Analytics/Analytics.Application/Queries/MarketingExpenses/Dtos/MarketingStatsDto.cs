namespace Analytics.Application.Queries.MarketingExpenses.Dtos;

public record MarketingStatsDto (
     decimal TotalAdSpend,  
     decimal Cac,            
     decimal Romi,          
     decimal ConversionRate);