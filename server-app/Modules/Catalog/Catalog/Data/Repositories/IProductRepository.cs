using Catalog.Products.Dtos;

namespace Catalog.Data.Repositories
{
    // Product aggregate access boundary. CachedProductRepository decorates this via Scrutor.
    // Reads that return DTO are cache-friendly; reads that return Product entity are tracked (for updates).
    public interface IProductRepository
    {
        // Cacheable — returns a projected DTO (safe to serialize, no navigation cycles).
        Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Not cached — returns a tracked entity with attributes loaded for aggregate mutation.
        Task<Product?> GetProductForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> ExistsBySkuAsync(string sku, Guid? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);

        Task AddAsync(Product product, CancellationToken cancellationToken = default);

        // Passing productId lets the cached decorator invalidate the correct key after a successful save.
        Task<int> SaveChangesAsync(Guid? productId = null, CancellationToken cancellationToken = default);
    }
}
