using Analytics.Application.Caching;
using Analytics.Application.Contracts;
using Analytics.Application.Queries.Dashboard.Enums;
using Analytics.Application.Queries.Marketing.GetMarketingDashboard.Dtos;
using FluentResults;

namespace Analytics.Application.Queries.Marketing.GetMarketingDashboard;

public class GetMarketingDashboardQuery(DateTime fromDate, DateTime toDate, Granularity granularity) 
    : QueryBase<Result<MarketingDashboardDto>>, ICacheableQuery
{
    public DateTime? FromDate { get; } = fromDate;
    public DateTime? ToDate { get; } = toDate;
    public Granularity Granularity { get; } = granularity;

    public string CacheKeyIdentifier => $"marketing-dashboard:get-marketing-dashboard";
    public TimeSpan CacheTtl => TimeSpan.FromMinutes(5);
}