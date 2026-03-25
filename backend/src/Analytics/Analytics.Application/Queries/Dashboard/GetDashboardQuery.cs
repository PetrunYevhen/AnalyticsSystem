using Analytics.Application.Caching;
using Analytics.Application.Contracts;
using Analytics.Application.Queries.Dashboard.Dtos;
using Analytics.Application.Queries.Dashboard.Enums;
using FluentResults;

namespace Analytics.Application.Queries.Dashboard;

public class GetDashboardQuery(DateTime? fromDate, DateTime? toDate, Granularity? granularity) 
    : QueryBase<Result<DashboardStatsResponse>>, ICacheableQuery
{
    public DateTime? FromDate { get; } = fromDate;
    public DateTime? ToDate { get; } = toDate;
    public Granularity? Granularity { get; } = granularity;
    
    public string CacheKeyIdentifier => $"dashboard:get-dashboard";
    public TimeSpan CacheTtl => TimeSpan.FromMinutes(5); 
}