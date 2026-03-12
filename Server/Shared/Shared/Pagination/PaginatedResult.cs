namespace Shared.Pagination;

public class PaginatedResult<TEntity>(IEnumerable<TEntity> items, long count, int pageNumber, int pageSize)
{
    public IEnumerable<TEntity> Items { get; } = items;
    public long Count { get; } = count;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
}