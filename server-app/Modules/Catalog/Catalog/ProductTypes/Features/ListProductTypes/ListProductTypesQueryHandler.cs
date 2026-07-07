using Catalog.ProductTypes.Dtos;

namespace Catalog.ProductTypes.Features.ListProductTypes
{
    public record ListProductTypesQuery(PaginationRequest PaginationRequest, string? Search)
        : IQuery<ListProductTypesResult>;

    public record ListProductTypesResult(PaginatedResult<ProductTypeSummaryDto> ProductTypes);

    public class ListProductTypesQueryHandler(CatalogDbContext dbContext)
        : IQueryHandler<ListProductTypesQuery, ListProductTypesResult>
    {
        public async Task<ListProductTypesResult> Handle(ListProductTypesQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var q = dbContext.ProductTypes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim().ToLower();
                q = q.Where(x => x.Code.Contains(s) || x.Name.ToLower().Contains(s));
            }

            var totalCount = await q.LongCountAsync(cancellationToken);

            var items = await q
                .OrderBy(x => x.Name)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(x => new ProductTypeSummaryDto(x.Id, x.Code, x.Name))
                .ToListAsync(cancellationToken);

            return new ListProductTypesResult(
                new PaginatedResult<ProductTypeSummaryDto>(pageIndex, pageSize, totalCount, items));
        }
    }
}
