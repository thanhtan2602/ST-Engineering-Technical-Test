namespace Shared.Pagination
{
    public class PaginatedResult<TEntity>
        (int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
        where TEntity : class
    {
        public int PageIndex { get; } = pageIndex;
        public int PageSize { get; } = pageSize;
        public long Count { get; } = count;
        public int TotalPages { get; } = pageSize > 0 ? (int)Math.Ceiling((double)count / pageSize) : 0;
        public IEnumerable<TEntity> Data { get; } = data;
    }
}
