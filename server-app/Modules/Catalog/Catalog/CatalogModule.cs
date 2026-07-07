using Catalog.Data;
using Catalog.Data.Repositories;
using Catalog.Data.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Data.Interceptors;
using Shared.Data.Seed;

namespace Catalog
{
    public static class CatalogModule
    {
        public static IServiceCollection AddCatalogModule(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Distributed cache: Redis in prod (ConnectionStrings:Redis), in-memory fallback for local dev.
            var redisConnection = configuration.GetConnectionString("Redis");
            if (!string.IsNullOrWhiteSpace(redisConnection))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnection;
                    options.InstanceName = "catalog:";
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            // Repository (Proxy pattern) + Cached decorator wired via Scrutor Decorate.
            services.AddScoped<IProductRepository, ProductRepository>();
            services.Decorate<IProductRepository, CachedProductRepository>();

            // Data - Infrastructure services
            var connectionString = configuration.GetConnectionString("Database");

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

            services.AddDbContext<CatalogDbContext>(options =>
            {
                options.AddInterceptors(new AuditableEntityInterceptor());
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IDataSeeder, CatalogDataSeeder>();

            return services;
        }

        public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
        {
            app.UseMigration<CatalogDbContext>();
            return app;
        }
    }
}
