using Analytics.Application.Caching;
using Analytics.Application.Common;
using Analytics.Application.Common.Sort;
using Analytics.Application.Contracts;
using Analytics.Application.Queries.Customers.GetAllCustomers.Dtos;

namespace Analytics.Application.Queries.Customers.GetAllCustomers;

public class GetAllCustomersQuery : QueryBase<PagedResult<CustomerDto>>, ICacheableQuery
{
    public int Page = 1;
    public int PageSize = 50;
    public CustomerSortFields SortBy {get;set;}
    public SortDirection Direction {get;set;}
    public string? Search { get; init; }         
    public bool? WithoutCampaign { get; init; }

    public GetAllCustomersQuery(int page, int pageSize, CustomerSortFields sortBy, SortDirection direction, string? search, bool? withoutCampaign)
    {
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy;
        Direction = direction;
        Search = search;
        WithoutCampaign = withoutCampaign;
    }

    public string CacheKeyIdentifier => "customers:get-all-customers";
    public TimeSpan CacheTtl => TimeSpan.FromMinutes(5);
}