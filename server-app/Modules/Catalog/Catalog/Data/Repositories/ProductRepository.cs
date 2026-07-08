using Catalog.Products.Dtos;

namespace Catalog.Data.Repositories
{
    // Real implementation: EF Core against CatalogDbContext.
    // CachedProductRepository wraps this via Scrutor Decorate.
    public class ProductRepository(CatalogDbContext dbContext) : IProductRepository
    {
        public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var p = await dbContext.Products
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.ProductType)
                .Include(x => x.Attributes).ThenInclude(a => a.AttributeDefinition)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (p is null) return null;

            var xminRaw = await dbContext.Database
                .SqlQuery<long>($"SELECT CAST(xmin AS bigint) FROM catalog.\"Products\" WHERE \"Id\" = {id}")
                .FirstAsync(cancellationToken);
            var xmin = (uint)xminRaw;

            return new ProductDto(
                p.Id, p.Sku, p.Name, p.Slug, p.Description, p.Price, p.Status.ToString(),
                p.CategoryId, p.Category.Name,
                p.BrandId, p.Brand.Name,
                p.ProductTypeId, p.ProductType.Name,
                xmin,
                p.CreatedAt,
                p.LastModifiedAt,
                p.Attributes
                    .Select(a => new ProductAttributeValueDto(
                        a.AttributeDefinitionId,
                        a.AttributeDefinition.Key,
                        a.AttributeDefinition.Label,
                        a.Value,
                        a.AttributeDefinition.DataType.ToString()))
                    .ToList(),
                p.Images
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new ProductImageDto(i.Id, i.Url, i.Alt, i.DisplayOrder, i.IsPrimary))
                    .ToList());
        }

        public Task<Product?> GetProductForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
            => dbContext.Products
                .Include(p => p.Attributes)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public Task<bool> ExistsBySkuAsync(string sku, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var normalized = sku.Trim();
            return excludeId.HasValue
                ? dbContext.Products.AnyAsync(p => p.Id != excludeId.Value && p.Sku == normalized, cancellationToken)
                : dbContext.Products.AnyAsync(p => p.Sku == normalized, cancellationToken);
        }

        public Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var normalized = slug.Trim().ToLower();
            return excludeId.HasValue
                ? dbContext.Products.AnyAsync(p => p.Id != excludeId.Value && p.Slug == normalized, cancellationToken)
                : dbContext.Products.AnyAsync(p => p.Slug == normalized, cancellationToken);
        }

        public Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            dbContext.Products.Add(product);
            return Task.CompletedTask;
        }

        public Task<int> SaveChangesAsync(Guid? productId = null, CancellationToken cancellationToken = default)
            => dbContext.SaveChangesAsync(cancellationToken);
    }
}
