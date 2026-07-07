using System.Text.Json;
using System.Text.Json.Serialization;
using Catalog.Products.Dtos;
using Microsoft.Extensions.Caching.Distributed;

namespace Catalog.Data.Repositories
{
    // Decorator: adds distributed-cache read-through for Product reads and cache eviction on writes.
    // Cache stores ProductDto (record → System.Text.Json handles it natively — no custom converter needed).
    public class CachedProductRepository(IProductRepository inner, IDistributedCache cache) : IProductRepository
    {
        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private static readonly DistributedCacheEntryOptions CacheEntryOptions = new()
        {
            AbsoluteExpirationRelativeToNow = Ttl
        };

        // Bump version when ProductDto shape changes to auto-invalidate stale cache entries.
        private const string CacheVersion = "v2";
        private static string Key(Guid id) => $"catalog:product:{CacheVersion}:{id}";

        public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cached = await cache.GetStringAsync(Key(id), cancellationToken);
            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<ProductDto>(cached, JsonOptions);

            var dto = await inner.GetProductByIdAsync(id, cancellationToken);
            if (dto is not null)
            {
                await cache.SetStringAsync(
                    Key(id),
                    JsonSerializer.Serialize(dto, JsonOptions),
                    CacheEntryOptions,
                    cancellationToken);
            }
            return dto;
        }

        // Tracked reads must always hit DB — never cache.
        public Task<Product?> GetProductForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
            => inner.GetProductForUpdateAsync(id, cancellationToken);

        public Task<bool> ExistsBySkuAsync(string sku, Guid? excludeId = null, CancellationToken cancellationToken = default)
            => inner.ExistsBySkuAsync(sku, excludeId, cancellationToken);

        public Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default)
            => inner.ExistsBySlugAsync(slug, excludeId, cancellationToken);

        public Task AddAsync(Product product, CancellationToken cancellationToken = default)
            => inner.AddAsync(product, cancellationToken);

        public async Task<int> SaveChangesAsync(Guid? productId = null, CancellationToken cancellationToken = default)
        {
            var result = await inner.SaveChangesAsync(productId, cancellationToken);
            if (productId.HasValue)
                await cache.RemoveAsync(Key(productId.Value), cancellationToken);
            return result;
        }
    }
}
