using Analytics.Application.Caching;
using Analytics.Application.Common.Sort;
using Analytics.Application.Contracts;
using Analytics.Application.Queries.Transactions.GetTransactionsDashboard.Dtos;

namespace Analytics.Application.Queries.Transactions.GetTransactionsDashboard;

public class GetTransactionsDashboardQuery : QueryBase<TransactionsDashboardDto>, 
    ICacheableQuery
{
    public int Page ;
    public int PageSize = 50;
    public TransactionSortFields SortBy {get;set;}
    public SortDirection Direction {get;set;}
    public string? Search { get;set;}

    public GetTransactionsDashboardQuery(int page, int pageSize, TransactionSortFields sortBy, SortDirection direction, string? search = null)
    {
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy;
        Direction = direction;
        Search = search;
    }
    
    public string CacheKey => $"transactions-dashboard:p{Page}:ps{PageSize}:s{SortBy}:d{Direction}:q{Search}";

    public string CacheKeyIdentifier => $"transactions-dashboard:get-transactions-dashboard";
    public TimeSpan CacheTtl => TimeSpan.FromMinutes(5);
}