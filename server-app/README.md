# Fashion Shop — Backend (Task 4)

.NET 8 modular monolith exposing product management APIs for the fashion-shop catalog.
Consumed by [client-app/](../client-app) and by the Postman collection in [docs/postman/](../docs/postman/).

## Tech stack

| Concern | Choice | Rationale |
| --- | --- | --- |
| Runtime | .NET 8 (LTS) | Long-term support, minimal APIs, `IExceptionHandler`, AOT-ready primitives. |
| Web framework | ASP.NET Core Minimal APIs + **Carter** | Endpoint discovery per assembly matches the modular monolith layout. |
| CQRS | **MediatR** + custom `ICommand<T>` / `IQuery<T>` markers | Lets `ValidationBehavior` be constrained to commands only. |
| ORM | **EF Core 8** + **Npgsql** | LINQ, per-module migrations, `xmin` concurrency, JSONB support. |
| Mapping | **Mapster** | Zero-config `Adapt<T>()`, faster than AutoMapper. |
| Validation | **FluentValidation** | Runs in `ValidationBehavior`; one validator per command. |
| Cache | **`IDistributedCache`** — Redis in prod, in-memory fallback | Scrutor-decorated `IProductRepository` keeps handlers cache-agnostic. |
| Docs | **Swashbuckle (Swagger)** | Auto-generated OpenAPI 3.0 at `/swagger`. |
| Errors | ASP.NET 8 **`IExceptionHandler`** + RFC 7807 Problem Details | Global handler maps typed exceptions to HTTP responses. |
| Container | **Docker Compose** (Postgres 17 + Redis 7 + API) | One command to boot the whole stack. |

## Prerequisites

- .NET 8 SDK — https://dotnet.microsoft.com/download
- Docker Desktop (for `docker compose up`) — https://www.docker.com/products/docker-desktop
- Optional: `dotnet-ef` global tool for migrations (`dotnet tool install -g dotnet-ef`)

## Quick start

### Option A — Docker Compose (recommended)

```bash
cd server-app
docker compose up --build
```

Brings up **Postgres 17** (host port `5434`), **Redis 7** (host port `6380`), and the **API** (host port `5005`).

Swagger UI: http://localhost:5005/swagger

### Option B — Local `dotnet run`

Start only the database:

```bash
cd server-app
docker compose up -d fashionshopdb cacheredis
```

Then run the API from the host:

```bash
dotnet run --project Bootstrapper/Api
```

Swagger UI: http://localhost:5005/swagger (or https://localhost:5050/swagger)

The default connection string in [Bootstrapper/Api/appsettings.json](Bootstrapper/Api/appsettings.json) already targets `localhost:5434`, so no config changes are needed.

## Configuration

Configuration is read from `appsettings.json`, `appsettings.<Environment>.json`, and environment variables (env-vars win). See [.env.example](.env.example) for the full list.

| Variable | Default | Purpose |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | `Development` | Standard ASP.NET Core env. |
| `ConnectionStrings__Database` | `Server=localhost;Port=5434;Database=FashionShopDb;User Id=postgres;Password=postgres;Include Error Detail=true` | Postgres DSN. |
| `ConnectionStrings__Redis` | `localhost:6380` | Redis DSN. Leave empty to force in-memory cache. |

## Migrations & seed

Migrations run **automatically on startup** via `UseMigration<CatalogDbContext>()` (see [Shared/Data/Extentions.cs](Shared/Shared/Data/Extentions.cs)). After migrations, the DI container resolves every `IDataSeeder` and calls `SeedAllAsync()` — seeders are idempotent (they check existence before inserting).

Current seed dataset ([Modules/Catalog/Catalog/Data/Seed/InitialData.cs](Modules/Catalog/Catalog/Data/Seed/InitialData.cs)):

- 5 categories
- 5 brands
- 15 attribute definitions (Color, Size, Material, Gender, Season, Fit, Style, Pattern, Closure, Sole, Waterproof, Capacity, Strap Type, Dimensions, Weight)
- 3 product types (Apparel, Footwear, Accessories) — each attaches a curated subset of the attributes above with `IsRequired` flags
- 31 products with attributes + images

### Add a migration

```bash
dotnet ef migrations add <Name> \
  --project Modules/Catalog/Catalog \
  --startup-project Bootstrapper/Api \
  --context CatalogDbContext \
  -o Data/Migrations
```

Do **not** run `database update` — the next `dotnet run` will apply it.

## Project structure

```
server-app/
├── Bootstrapper/Api/                ASP.NET Core host
│   ├── Program.cs                   Wires Carter + MediatR + modules
│   ├── appsettings.json
│   ├── Dockerfile
│   └── wwwroot/uploads/products/    Runtime uploads (git-ignored)
├── Modules/
│   ├── Catalog/Catalog/             Implemented module
│   │   ├── CatalogModule.cs         AddCatalogModule / UseCatalogModule
│   │   ├── GlobalUsing.cs
│   │   ├── Products/                Aggregate root — CRUD + attributes + images
│   │   │   ├── Features/            One folder per use case (Endpoint + Handler)
│   │   │   ├── Models/              Product, ProductAttribute, ProductImage, ProductStatus
│   │   │   ├── Dtos/
│   │   │   ├── Exceptions/
│   │   │   └── Services/
│   │   ├── Categories/              Reference data
│   │   ├── Brands/
│   │   ├── ProductTypes/            EAV template
│   │   ├── AttributeDefinitions/    EAV catalog
│   │   └── Data/
│   │       ├── CatalogDbContext.cs
│   │       ├── Configurations/      EF Core IEntityTypeConfiguration<>
│   │       ├── Migrations/          One initial migration
│   │       ├── Repositories/        IProductRepository + CachedProductRepository (Scrutor decorator)
│   │       └── Seed/
│   ├── Basket/                      Empty skeleton (reserved)
│   └── Ordering/                    Empty skeleton (reserved)
├── Shared/Shared/                   Cross-cutting building blocks
│   ├── Behaviors/                   ValidationBehavior, LoggingBehavior
│   ├── CQRS/                        ICommand<T>, IQuery<T> + handlers
│   ├── DDD/                         Entity<T>, AuditableSoftDeleteEntity<T>
│   ├── Data/                        Migration bootstrap, AuditableEntityInterceptor
│   ├── Exceptions/                  Base exceptions + CustomExceptionHandler
│   ├── Extensions/                  Carter + MediatR DI helpers
│   └── Pagination/                  PaginatedResult<T>
├── docker-compose.yml
├── docker-compose.override.yml
├── fashion-shop.slnx
└── README.md
```

## Endpoint inventory

All endpoints are mounted under `/api/v1/…`.

### Products (`/api/v1/products`)

| Method | Path | Purpose | Cached | If-Match |
| --- | --- | --- | --- | --- |
| GET | `/` | List with filter + sort + pagination | ✅ | — |
| GET | `/{id}` | Detail (attributes + images) — response header `ETag` | ✅ | — |
| POST | `/` | Create | — | — |
| PUT | `/{id}` | Update | — | ✅ |
| PATCH | `/{id}/status` | Change status | — | ✅ |
| DELETE | `/{id}` | Soft delete | — | ✅ |
| POST | `/{id}/attributes` | Upsert attribute value | — | ✅ |
| DELETE | `/{id}/attributes/{attributeDefinitionId}` | Remove attribute | — | ✅ |
| POST | `/{id}/images` (multipart) | Upload image | — | — |
| DELETE | `/{id}/images/{imageId}` | Remove image | — | — |

List filters: `search`, `categoryId`, `brandId`, `productTypeId`, `status`, `minPrice`, `maxPrice`, `sort` (`price_asc`, `price_desc`, `name_asc`, `name_desc`, `newest`).

### Reference data

`/api/v1/categories`, `/api/v1/brands`, `/api/v1/product-types`, `/api/v1/attribute-definitions` — full CRUD each.

## Cross-cutting behavior

- **Validation** — every command goes through `ValidationBehavior<TRequest, TResponse>` (constrained `where TRequest : ICommand<TResponse>`). Queries are **not** validated by the pipeline; put input checks in the query handler if needed.
- **Auditing** — [AuditableEntityInterceptor](Shared/Shared/Data/Interceptors/AuditableEntityInterceptor.cs) populates `CreatedAt`, `CreatedBy`, `LastModifiedAt`, `LastModifiedBy` on any entity deriving from `Entity<T>`. Audit user is hardcoded as `"system"` for the test.
- **Soft delete** — `Product` derives from `AuditableSoftDeleteEntity<Guid>`. A global query filter (`HasQueryFilter(p => p.DeletedAt == null)`) hides deleted rows from every read path.
- **Optimistic concurrency** — `Product.RowVersion` maps to Postgres `xmin` via `UseXminAsConcurrencyToken()`. Writes accept `If-Match: "<rowVersion>"`; a stale token surfaces as `DbUpdateConcurrencyException` → 409 in the exception handler.
- **Global exception handler** — [CustomExceptionHandler](Shared/Shared/Exceptions/Handler/CustomExceptionHandler.cs) maps `ValidationException` (400), `NotFoundException` (404), `BusinessRuleException` (422), `DbUpdateConcurrencyException` (409) to RFC 7807 Problem Details.

## Error contract (RFC 7807)

| HTTP | Title | Cause |
| --- | --- | --- |
| 400 | `Validation Failed` | FluentValidation error. Body includes `errors: { field: [msg] }`. |
| 400 | `Bad Request` | `BadRequestException` (e.g. malformed multipart upload). |
| 404 | `Not Found` | Module-specific `NotFoundException` (e.g. `ProductNotFoundException`). |
| 409 | `Concurrency Conflict` | `DbUpdateConcurrencyException` — stale `If-Match`. Body includes the current row version. |
| 422 | `Business Rule Violation` | Cross-aggregate rule (e.g. delete Category with products). |
| 500 | `Internal Server Error` | Unhandled — logged with request path. |

Every response body follows RFC 7807 with `type`, `title`, `status`, `detail`, `instance`, and framework-supplied `traceId`.

## Adding a new feature (convention)

1. Create `Modules/Catalog/Catalog/<Aggregate>/Features/<UseCase>/`.
2. Add `<UseCase>Endpoint.cs` implementing `ICarterModule` — auto-discovered.
3. Add `<UseCase>CommandHandler.cs` (or `QueryHandler.cs`) implementing `ICommandHandler<TCommand, TResult>`.
4. If it's a command, add `<UseCase>CommandValidator : AbstractValidator<TCommand>` in the same file — auto-registered.
5. Naming conventions used across the codebase:
   - HTTP DTOs: `<UseCase>Request`, `<UseCase>Response`
   - CQRS types: `<UseCase>Command` / `<UseCase>Query`, `<UseCase>Result`
6. Domain invariants live in aggregate `Create` / `Update` methods, not in handlers or validators.

## Limitations

- Auth is stubbed (`"system"`). Future: JWT + `ICurrentUser` + interceptor update.
- No automated tests. Future: `Product.Create` unit tests, handler integration tests with Postgres testcontainers.
- Cache invalidation is per-instance (Redis is shared but no keyspace notification). Horizontal scaling needs pub/sub.
- Images stored on local disk (`wwwroot/uploads/products/`). Future: S3-compatible object storage + CDN.
- No structured logging (Serilog documented as a future upgrade).
- No health check endpoint. Future: `/health` with `AddDbContextCheck<CatalogDbContext>()`.

See [../docs/report/report.md](../docs/report/report.md) for the full discussion.