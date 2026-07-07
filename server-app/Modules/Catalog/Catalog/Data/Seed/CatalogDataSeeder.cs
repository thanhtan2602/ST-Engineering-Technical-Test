using Microsoft.EntityFrameworkCore;
using Shared.Data.Seed;

namespace Catalog.Data.Seed
{
    public class CatalogDataSeeder(CatalogDbContext dbContext) : IDataSeeder
    {
        public async Task SeedAllAsync()
        {
            await SeedIfEmptyAsync(dbContext.Categories, InitialData.Categories);
            await SeedIfEmptyAsync(dbContext.Brands, InitialData.Brands);
            await SeedIfEmptyAsync(dbContext.AttributeDefinitions, InitialData.AttributeDefinitions);
            await SeedIfEmptyAsync(dbContext.ProductTypes, InitialData.ProductTypes);
            await SeedIfEmptyAsync(dbContext.Products, InitialData.Products);
        }

        private async Task SeedIfEmptyAsync<T>(DbSet<T> set, IEnumerable<T> data) where T : class
        {
            if (await set.AnyAsync()) return;
            await set.AddRangeAsync(data);
            await dbContext.SaveChangesAsync();
        }
    }
}
