using Analytics.Application.Caching;
using Analytics.Application.Common.Sort;
using Analytics.Application.Contracts;
using Analytics.Application.Queries.Orders.GetOrdersDashboard.Dtos;

namespace Analytics.Application.Queries.Orders.GetOrdersDashboard;

public class GetOrdersDashboardQuery : QueryBase<OrdersDashboardDto>, ICacheableQuery
{
    public int Page ;
    public int PageSize = 50;
    public OrderSortFields SortBy {get;set;}
    public SortDirection Direction {get;set;}
    public string? Search {get;set;}

    public GetOrdersDashboardQuery(int page, int pageSize, OrderSortFields sortBy, SortDirection direction, string? search = null)
    {
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy;
        Direction = direction;
        Search = search;
    }

    public string CacheKeyIdentifier => $"orders-dashboard:get-orders-dashboard";
    public TimeSpan CacheTtl => TimeSpan.FromMinutes(5);
}