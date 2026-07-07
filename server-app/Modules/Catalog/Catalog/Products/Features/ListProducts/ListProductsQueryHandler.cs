using Catalog.Products.Dtos;

namespace Catalog.Products.Features.ListProducts
{
    public enum ProductSortOrder { Newest, NameAsc, NameDesc, PriceAsc, PriceDesc }

    public record ListProductsQuery(
        PaginationRequest PaginationRequest,
        string? Search,
        Guid? CategoryId,
        Guid? BrandId,
        Guid? ProductTypeId,
        ProductStatus? Status,
        decimal? MinPrice,
        decimal? MaxPrice,
        ProductSortOrder Sort = ProductSortOrder.Newest) : IQuery<ListProductsResult>;

    public record ListProductsResult(PaginatedResult<ProductSummaryDto> Products);

    public class ListProductsQueryHandler(CatalogDbContext dbContext)
        : IQueryHandler<ListProductsQuery, ListProductsResult>
    {
        public async Task<ListProductsResult> Handle(ListProductsQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = Math.Max(0, query.PaginationRequest.PageIndex);
            var pageSize = Math.Clamp(query.PaginationRequest.PageSize, 1, 100);

            var q = dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductType)
                .Include(p => p.Images)
                .AsQueryable();

            if (query.CategoryId.HasValue)
                q = q.Where(p => p.CategoryId == query.CategoryId.Value);
            if (query.BrandId.HasValue)
                q = q.Where(p => p.BrandId == query.BrandId.Value);
            if (query.ProductTypeId.HasValue)
                q = q.Where(p => p.ProductTypeId == query.ProductTypeId.Value);
            if (query.Status.HasValue)
                q = q.Where(p => p.Status == query.Status.Value);
            if (query.MinPrice.HasValue)
                q = q.Where(p => p.Price >= query.MinPrice.Value);
            if (query.MaxPrice.HasValue)
                q = q.Where(p => p.Price <= query.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim().ToLower();
                q = q.Where(p =>
                    p.Name.ToLower().Contains(s) ||
                    p.Sku.ToLower().Contains(s) ||
                    p.Slug.Contains(s));
            }

            q = query.Sort switch
            {
                ProductSortOrder.NameAsc   => q.OrderBy(p => p.Name),
                ProductSortOrder.NameDesc  => q.OrderByDescending(p => p.Name),
                ProductSortOrder.PriceAsc  => q.OrderBy(p => p.Price),
                ProductSortOrder.PriceDesc => q.OrderByDescending(p => p.Price),
                _                          => q.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = await q.LongCountAsync(cancellationToken);

            var items = await q
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(p => new ProductSummaryDto(
                    p.Id,
                    p.Sku,
                    p.Name,
                    p.Slug,
                    p.Price,
                    p.Status.ToString(),
                    p.CategoryId, p.Category.Name,
                    p.BrandId, p.Brand.Name,
                    p.ProductTypeId, p.ProductType.Name,
                    p.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault()))
                .ToListAsync(cancellationToken);

            return new ListProductsResult(new PaginatedResult<ProductSummaryDto>(pageIndex, pageSize, totalCount, items));
        }
    }
}
