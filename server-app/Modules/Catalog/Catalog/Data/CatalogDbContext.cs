using System.Reflection;
using Catalog.AttributeDefinitions.Models;
using Catalog.Brands.Models;
using Catalog.Categories.Models;
using Catalog.ProductTypes.Models;
using Catalog.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Data
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<ProductType> ProductTypes => Set<ProductType>();
        public DbSet<ProductTypeAttribute> ProductTypeAttributes => Set<ProductTypeAttribute>();
        public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("catalog");
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
    }
}
