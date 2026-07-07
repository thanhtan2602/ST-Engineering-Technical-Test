using Catalog.Categories.Dtos;

namespace Catalog.Categories.Features.ListCategories
{
    public record ListCategoriesQuery(PaginationRequest PaginationRequest, string? Search, Guid? ParentId)
        : IQuery<ListCategoriesResult>;

    public record ListCategoriesResult(PaginatedResult<CategoryDto> Categories);

    public class ListCategoriesQueryHandler(CatalogDbContext dbContext)
        : IQueryHandler<ListCategoriesQuery, ListCategoriesResult>
    {
        public async Task<ListCategoriesResult> Handle(ListCategoriesQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var q = dbContext.Categories.AsNoTracking().AsQueryable();

            if (query.ParentId.HasValue)
                q = q.Where(c => c.ParentId == query.ParentId.Value);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim().ToLower();
                q = q.Where(c => c.Name.ToLower().Contains(s) || c.Slug.Contains(s));
            }

            var totalCount = await q.LongCountAsync(cancellationToken);

            var items = await q
                .OrderBy(c => c.Name)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.ParentId))
                .ToListAsync(cancellationToken);

            return new ListCategoriesResult(new PaginatedResult<CategoryDto>(pageIndex, pageSize, totalCount, items));
        }
    }
}
