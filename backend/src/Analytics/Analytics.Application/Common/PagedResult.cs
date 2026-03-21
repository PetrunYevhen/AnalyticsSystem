namespace Analytics.Application.Common;

public class PagedResult<T> where T : class
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public IReadOnlyList<T> Items { get; set; }
    
    
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page <  TotalPages;
    public bool HasPreviousPage => Page > 1;

    public PagedResult(int page, int pageSize, int totalCount, IReadOnlyList<T> items)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        Items = items;
    }
}