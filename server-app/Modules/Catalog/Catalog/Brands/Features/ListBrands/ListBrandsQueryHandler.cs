using Catalog.Brands.Dtos;

namespace Catalog.Brands.Features.ListBrands
{
    public record ListBrandsQuery(PaginationRequest PaginationRequest, string? Search)
        : IQuery<ListBrandsResult>;

    public record ListBrandsResult(PaginatedResult<BrandDto> Brands);

    public class ListBrandsQueryHandler(CatalogDbContext dbContext)
        : IQueryHandler<ListBrandsQuery, ListBrandsResult>
    {
        public async Task<ListBrandsResult> Handle(ListBrandsQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var q = dbContext.Brands.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim().ToLower();
                q = q.Where(b => b.Name.ToLower().Contains(s) || b.Slug.Contains(s));
            }

            var totalCount = await q.LongCountAsync(cancellationToken);

            var items = await q
                .OrderBy(b => b.Name)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(b => new BrandDto(b.Id, b.Name, b.Slug))
                .ToListAsync(cancellationToken);

            return new ListBrandsResult(new PaginatedResult<BrandDto>(pageIndex, pageSize, totalCount, items));
        }
    }
}
