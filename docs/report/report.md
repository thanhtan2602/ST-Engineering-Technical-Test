# Technical Report — Fashion Shop

**ST Engineering .NET Developer Test — Task 4 & Task 5.**

This report accompanies the source code in [server-app/](../../server-app/) and [client-app/](../../client-app/). It is structured around the **five evaluation criteria** stated in the ST Engineering .NET Developer Test rubric:

1. **Approach the requirement** — dev process is transparent (design → build → test → document).
2. **Database / project structure** — justified choices, extensible for future attributes.
3. **Technology stack components** — every ORM / validation / state / API client / uploader / UI framework is named and justified.
4. **API and data handling** — request validation, response shaping, contract clarity.
5. **Performance** — caching, concurrency, lazy loading, code splitting, error handling.

Sections 8–10 add limitations, future improvements, and a per-criterion self-assessment.

---

## Executive Summary

This submission delivers production-grade product management as a **.NET 8 modular monolith** paired with a **React 18 SPA**, prioritising schema-flexible attributes, strong-consistency writes under contention, and one-command boot over ceremony. It targets a senior review: every non-trivial choice (EAV vs. per-type columns, Scrutor decorator vs. `CachingBehavior` pipeline, `xmin` vs. maintained `byte[]` row version) is called out with the trade-off that motivated it.

- **Objective** — Build a Product Management system for a retail catalog that scales with new attribute types and preserves strong consistency on writes, without sacrificing developer velocity.
- **Solution** — Modular monolith (`Bootstrapper/Api` + `Modules/Catalog` + `Shared`) with **vertical slice per feature**, DDD aggregates, MediatR pipeline (`ValidationBehavior` on commands only), EF Core 8 on PostgreSQL 17 (**EAV** for extensible attributes, `xmin` for optimistic concurrency), Scrutor-decorated Redis cache. Frontend is Vite + React 18 + TS strict with TanStack Query for server state and Zustand for UI state.
- **Key Features** — Product / Category / Brand / Product Type / Attribute Definition CRUD · list filter + sort + pagination · **dynamic EAV attributes** (add new attribute = 1 INSERT, no migration) · image upload with primary-image invariant · **optimistic concurrency** (`ETag` / `If-Match`, stale → 409) · distributed cache (Redis, in-memory fallback) · RFC 7807 Problem Details error contract · auto-migration + idempotent seed on startup.
- **Differentiators** — (1) EAV design (`ProductType` + `ProductTypeAttribute` + `AttributeDefinition` + `ProductAttribute`) makes the catalog attribute-schemaless: adding *Waterproof Rating* tomorrow is one INSERT with zero downtime. (2) Every mutation is guarded by `ETag` / `If-Match` mapped to Postgres `xmin` — concurrent editors converge safely without pessimistic locking, and the SPA surfaces conflicts through a dedicated modal rather than a silent overwrite.
- **Deliverables** — Docker Compose stack (Postgres + Redis + API), Postman collection with a **Guided Demo Flow** folder, ERD (Mermaid + PNG), per-app READMEs, this report, SPA screenshots.
- **Non-goals** — Auth (audit user hardcoded `"system"`), automated tests, Basket / Ordering modules (skeleton `.csproj` only). Trade-offs and production wiring for each are captured in [§8 Limitations](#8-limitations) and [§9 Future improvements](#9-future-improvements).

---

## 1. Approach — dev process

The delivery followed a design → build → polish → document sequence:

1. **Scope & design** — freeze in/out, sketch the domain (aggregates, EAV template), decide the endpoint inventory.
2. **Backend** — domain aggregates, EAV, endpoints, exception handler, caching decorator, idempotent seed. Ships with Swagger + Postman collection.
3. **Frontend** — scaffold Vite + React + TS, HTTP client with ETag capture, list / detail / form pages, dynamic attribute editor, image dropzone.
4. **Polish** — Postman Guided Demo Flow, seed refinement, dark mode, 409 concurrency modal.
5. **Documentation** — this report, root + per-app READMEs, ERD (Mermaid + PNG), screenshots.

Scope in/out was frozen up front:

**In scope**
- Products / Categories / Brands / ProductTypes / AttributeDefinitions master data.
- CRUD + list with filter / sort / pagination.
- EAV-style attributes.
- Optimistic concurrency, distributed cache, image upload.
- SPA with full CRUD flow.

**Out of scope, documented**
- Auth / user management (audit user hardcoded `"system"`).
- Basket / Ordering (skeleton projects only).
- Automated tests.
- CI/CD beyond `docker compose up`.
- Multi-currency, i18n, multi-tenant.

---

## 2. Database & project structure

### 2.1 Why relational + Postgres

Product catalogs need ACID for price / status transitions, referential integrity between `Product ↔ Category / Brand / ProductType`, and rich filter queries. A document DB would trade those away for schema flexibility we do not need at this scale. Postgres specifically because:

- Free / open source.
- `xmin` gives us cheap optimistic concurrency (`UseXminAsConcurrencyToken()`) without maintaining a `rowversion` byte array.
- Rich index catalog: partial unique, GIN, expression, `pg_trgm` for future full-text.
- JSONB is available as an escape hatch if EAV becomes limiting.

### 2.2 Extensibility — EAV over per-type columns

The rubric explicitly asks for "extensibility for future attributes / features". The design separates **shape** from **data**:

```
ProductType ── ProductTypeAttribute ──▶ AttributeDefinition
    │                                      ▲
    │  (each Product has a ProductType)    │
    ▼                                      │
Product ── ProductAttribute ───────────────┘
                │
                └── Value (string, canonicalized)
```

- **`AttributeDefinition`** — the catalog of possible attributes (`Color`, `Size`, `Material`, …). Each has a `DataType` (`String`, `Number`, `Boolean`, `Date`, `Enum`) and, for `Enum`, an allowed-values list.
- **`ProductType`** — a template (e.g. `Fashion`) that declares which attributes are **required** vs **optional** through `ProductTypeAttribute.IsRequired`.
- **`ProductAttribute`** — the concrete value for a given `Product × AttributeDefinition`. Values are stored as strings and cast at read time.

**Adding a new attribute** (say `Waterproof Rating`) is a single `INSERT` into `AttributeDefinition` plus optional links via `ProductTypeAttribute`. **No migration required.**

**Trade-off**: EAV loses column-level type checking and makes ad-hoc SQL analytics harder. Mitigations:

1. `AttributeDefinition.DataType` is enforced in the command handler when writing values.
2. A JSONB fallback column `Product.AttributesJson` can be added later without disturbing existing rows if analytics teams complain.

### 2.3 Core schema (schema `catalog`)

See [../erd/README.md](../erd/README.md) for the full ERD.

| Table | Purpose | Notes |
| --- | --- | --- |
| `products` | Aggregate root | `Sku`, `Slug` — partial unique (`WHERE deleted_at IS NULL`); `RowVersion` (`xmin`); soft-delete columns; FK to Category / Brand / ProductType. |
| `product_attributes` | EAV row | Unique `(ProductId, AttributeDefinitionId)`. |
| `product_images` | Media | At most one `IsPrimary = true` per product — enforced in the aggregate (`AddImage` demotes the previous primary). |
| `categories` | Taxonomy | Self-referential `ParentId`. |
| `brands`, `product_types`, `attribute_definitions` | Reference data | Slug / Code unique. |
| `product_type_attributes` | EAV template | Composite PK `(ProductTypeId, AttributeDefinitionId)` + `IsRequired`. |

Indexes (see [Modules/Catalog/Catalog/Data/Configurations/ProductConfiguration.cs](../../server-app/Modules/Catalog/Catalog/Data/Configurations/ProductConfiguration.cs)):

- FK indexes: `products.CategoryId`, `products.BrandId`, `products.ProductTypeId`.
- Filter: `products.Status`, composite `(Status, CategoryId)`.
- Partial unique: `products.Sku`, `products.Slug` — both `WHERE deleted_at IS NULL`.
- Case-insensitive search: `products.Name` btree today; `pg_trgm` GIN is documented as a future upgrade.

### 2.4 Concurrency & consistency

- **Optimistic concurrency**: `Product.RowVersion` (`xmin`) is checked on every write. Stale `If-Match` → `DbUpdateConcurrencyException` → 409 in [CustomExceptionHandler](../../server-app/Shared/Shared/Exceptions/Handler/CustomExceptionHandler.cs).
- **Strong consistency inside a request**: everything happens in a single `SaveChangesAsync` transaction; no eventual-consistency stores in the write path.
- **Cross-aggregate operations** (e.g. delete `Category` that has products) are guarded at the handler level (`BusinessRuleException` → 422) — no cascading deletes.

### 2.5 Soft delete

`Product : AuditableSoftDeleteEntity<Guid>`. A **global query filter** (`HasQueryFilter(p => p.DeletedAt == null)`) hides deleted rows from every read path. `.IgnoreQueryFilters()` is reserved for a future admin restore endpoint.

### 2.6 Project structure — modular monolith

```
server-app/
├── Bootstrapper/Api/     — single ASP.NET host, wires modules
├── Modules/Catalog/      — implemented module (vertical slice per feature)
├── Modules/Basket/       — empty skeleton
├── Modules/Ordering/     — empty skeleton
└── Shared/               — cross-cutting: CQRS, DDD, interceptors, exception handler
```

- Modules **never reference each other** — verified by inspecting each `.csproj`.
- Each module has one `DbContext` and its own migration folder.
- Endpoints are `ICarterModule` implementations, auto-discovered by assembly.
- Feature layout: `Products/Features/<UseCase>/{Endpoint,Handler}.cs` — one folder per HTTP endpoint.

### 2.7 Schema-per-module boundary

Isolation in a modular monolith works on two axes: **code** (`.csproj` references) and **data** (database schema). Catalog wires the second axis via `HasDefaultSchema("catalog")` in [CatalogDbContext.OnModelCreating](../../server-app/Modules/Catalog/Catalog/Data/CatalogDbContext.cs):

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    builder.HasDefaultSchema("catalog");
    builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
```

All eight Catalog tables live in schema `catalog` (`catalog.products`, `catalog.product_attributes`, …) — nothing in `public`. When `Basket` and `Ordering` are implemented, they will get their own schemas (`basket`, `ordering`) with their own migrations, DbContexts, and seeders.

**Why this matters**:

- **Table namespace conflicts disappear.** A hypothetical `orders` table in Basket does not collide with an `orders` table an operator adds ad-hoc to `public`.
- **Grants can be scoped per module.** Read-only analytics users can be granted `USAGE, SELECT` on schema `catalog` alone without touching Basket data — cheaper than table-level GRANTs and impossible to forget when adding new tables.
- **Migration surface stays clean.** Each `DbContext` owns a schema, so `dotnet ef migrations add` for one module cannot accidentally scaffold tables from another.
- **Refactor to microservices later is cheaper.** If Basket is extracted, the schema move is `pg_dump --schema=basket … | psql <new-db>` rather than untangling shared tables in `public`.

The trade-off is one extra qualifier in ad-hoc SQL (`catalog.products` instead of `products`) and one extra config line per module. Both are acceptable for the isolation guarantee.

---

## 3. Technology stack — components & justification

### 3.1 Backend

| Concern | Choice | Why this over alternatives |
| --- | --- | --- |
| Runtime | .NET 8 | LTS through Nov 2026; `IExceptionHandler`, AOT-ready primitives. |
| Framework | ASP.NET Core Minimal APIs + Carter | Endpoint discovery per assembly matches the modular monolith — no manual `MapEndpoints` list per module. |
| CQRS | MediatR + custom `ICommand<T>` / `IQuery<T>` markers | Markers let `ValidationBehavior` be `where TRequest : ICommand<TResponse>` — queries stay unvalidated for perf. |
| ORM | EF Core 8 + Npgsql | LINQ, per-module migrations, `xmin` concurrency token, `IEntityTypeConfiguration<>` for clean config. |
| Mapping | Mapster | `Adapt<T>()` is zero-config for our request/response shapes; faster than AutoMapper on start-up. |
| Validation | FluentValidation | Runs in the pipeline; one validator per command; testable in isolation. |
| Cache | `IDistributedCache` + StackExchange.Redis + `AddDistributedMemoryCache` fallback | Handlers stay cache-agnostic — cache lives in [CachedProductRepository](../../server-app/Modules/Catalog/Catalog/Data/Repositories/CachedProductRepository.cs) via Scrutor `Decorate`. |
| Docs | Swashbuckle | OpenAPI 3.0 out-of-the-box at `/swagger`. |
| Errors | ASP.NET 8 `IExceptionHandler` + RFC 7807 Problem Details | Global handler maps `ValidationException`, `NotFoundException`, `BusinessRuleException`, `DbUpdateConcurrencyException`. |
| Container | Docker Compose (Postgres + Redis + API) | One command to boot the whole stack from a clean checkout. |

**Why Scrutor decorator instead of a `CachingBehavior` pipeline?** The decorator keeps the handler layer completely cache-agnostic and makes invalidation explicit (`RemoveAsync` on save). A `CachingBehavior` with a marker interface (`ICacheableQuery`) works too, but requires cache keys to be derivable from the request DTO — for filter-heavy list queries that becomes error-prone. Trade-off: decorator hits the DB layer once per query for the cache lookup, whereas a pipeline behavior can short-circuit the whole MediatR chain. For this workload it does not matter.

### 3.2 Frontend

| Concern | Choice | Why |
| --- | --- | --- |
| Build | Vite 5 | Instant HMR, minimal config, first-class TS. |
| Framework | React 18 | Concurrent rendering, Suspense for lazy routes. |
| Language | TypeScript 5.4 (`strict`) | Type safety end-to-end, `z.infer<>` gives us free form types. |
| Router | React Router 6 (data router) + `React.lazy` | File-based routing is overkill for a 4–6h test; explicit routes are clearer. |
| Server state | TanStack Query 5 | Cache, dedup, retry, invalidation — no boilerplate. |
| Client state | Zustand | UI-only state (theme, sidebar). Redux would be overkill. |
| HTTP client | axios + interceptors | ETag capture, RFC 7807 error parsing, future auth header. |
| Forms | react-hook-form + Zod (via `zodResolver`) | Uncontrolled inputs, minimal re-renders; `z.infer<>` doubles as TS types. |
| UI | Tailwind CSS + Radix UI primitives + shadcn-style components | Headless, accessible, no lock-in. |
| Uploads | react-dropzone | Drag-drop, preview, per-file progress, MIME + size validation. |
| Notifications | sonner | Clean, accessible toasts. |
| Tables | @tanstack/react-table | Headless; server-side pagination with a few hooks. |
| Package manager | pnpm | Fast, deterministic. |

---

## 4. API design & data handling

### 4.1 Conventions

- Base path `/api/v1/…`. Versioning in the URL segment.
- Resource-oriented: `products`, `categories`, `brands`, `product-types`, `attribute-definitions`.
- JSON body, `application/json`, camelCase.
- Pagination envelope: `{ pageIndex, pageSize, count, data }` (see [PaginatedResult<T>](../../server-app/Shared/Shared/Pagination/PaginatedResult.cs)).
- Errors: RFC 7807 Problem Details with framework-supplied `traceId`.
- Enums serialized as strings (`JsonStringEnumConverter`).

### 4.2 Endpoint inventory

Full list in [../../server-app/README.md](../../server-app/README.md#endpoint-inventory).

**Products** (10 endpoints): list, detail, create, update, patch-status, delete, upsert attribute, remove attribute, upload image, delete image.

**Reference data**: CRUD for categories, brands, product types (with attribute attach/detach), attribute definitions.

### 4.3 Request → Command flow

```
HTTP Request
  → Endpoint (Carter) — parse route + body into <UseCase>Request record
  → request.Adapt<Command>() (Mapster)
  → ISender.Send(command)
    → ValidationBehavior — FluentValidation (commands only)
    → LoggingBehavior — logs request + duration
    → CommandHandler — loads aggregate, invokes domain method, SaveChangesAsync
  → result.Adapt<Response>()
  → Results.Ok(response) / Results.Created(location, response)
```

### 4.4 Validation examples

- `CreateProductCommandValidator`: SKU non-empty, ≤ 64 chars, `^[A-Z0-9-]+$`; Name 1..200; Slug 1..220 kebab-case; Price ≥ 0; Category / Brand / ProductType IDs non-empty.
- `UpsertProductAttributeCommand`: `attributeDefinitionId` non-empty; `value` parses against `AttributeDefinition.DataType` (Enum values checked against `AllowedValues`, Number parsed as decimal, Boolean as `true/false`, Date as ISO-8601).
- Cross-field checks (Slug uniqueness) are done in the handler against the DB — the validator would double the round-trip.

### 4.5 Error contract (RFC 7807)

| HTTP | Title | Cause |
| --- | --- | --- |
| 400 | `Validation Failed` | FluentValidation. Body includes `errors: { field: [msg] }`. |
| 400 | `Bad Request` | Malformed request (e.g. multipart parse). |
| 404 | `Not Found` | Module-specific `NotFoundException`. |
| 409 | `Concurrency Conflict` | `DbUpdateConcurrencyException` — stale `If-Match`. |
| 422 | `Business Rule Violation` | Cross-aggregate rule (e.g. delete Category still in use). |
| 500 | `Internal Server Error` | Unhandled — logged. |

The SPA parses `application/problem+json` in [client-app/src/shared/api/problemDetails.ts](../../client-app/src/shared/api/problemDetails.ts) into an `ApiError { status, title, detail, errors, traceId, isConcurrencyConflict }` and dispatches:

- Field-level `errors` → `setError(field, …)` in react-hook-form.
- Toast summary via sonner.
- `isConcurrencyConflict` → 409 modal with **Stay here** / **Reload** buttons.

### 4.6 Optimistic concurrency round-trip

The reviewer can exercise this from Postman ("Guided Demo Flow" folder) or the SPA:

1. `GET /products/{id}` — response header `ETag: "<rowVersion>"`, body also carries `rowVersion`.
2. `PUT /products/{id}` with `If-Match: "<rowVersion>"` — succeeds, new `RowVersion` returned.
3. `PUT /products/{id}` with the **old** `If-Match` — 409 Problem Details.

The SPA captures the ETag from `GET /products/{id}` into a `Map<key, etag>` in `src/shared/api/etagStore.ts` and re-injects it on the next mutation.

---

## 5. Performance

### 5.1 Caching strategy

**Layer**: distributed cache via `IDistributedCache`, decorated onto `IProductRepository` with [Scrutor.Decorate](../../server-app/Modules/Catalog/Catalog/CatalogModule.cs). Redis when `ConnectionStrings:Redis` is set; in-memory otherwise.

**What is cached**: `GetProductByIdAsync` — the shape returned by the detail endpoint.

**Invalidation**: [CachedProductRepository](../../server-app/Modules/Catalog/Catalog/Data/Repositories/CachedProductRepository.cs) inspects the change-tracker in `SaveChangesAsync` and evicts the `catalog:product:{id}` key for any modified `Product`. No pub/sub — cache is local per instance.

**Trade-off (documented)**: list queries are not cached in this iteration. Adding a `CachingBehavior<TRequest, TResponse> : IPipelineBehavior` with a marker `ICacheableQuery` and a filter-serialization convention is a documented next step.

### 5.2 Query performance

- `AsNoTracking()` on all read paths (see [ListProductsQueryHandler](../../server-app/Modules/Catalog/Catalog/Products/Features/ListProducts/ListProductsQueryHandler.cs)).
- Projection to DTO via `.Select(new ProductSummaryDto { … })` for list — full aggregate is never loaded.
- Filters translate to SQL `WHERE` predicates via LINQ — verified in Npgsql logs.
- `AsSplitQuery()` for the detail path is a documented future upgrade (currently a single query with `Include` chain — acceptable for the seed size, but risks Cartesian growth as image / attribute counts grow).

### 5.3 Concurrency

Optimistic via `xmin` — see §2.4. No pessimistic locking on read.

### 5.4 Frontend performance

- **Code splitting**: every route is a `React.lazy` chunk; router auto-wraps in Suspense.
- **Query cache**: TanStack Query dedupes concurrent requests and keeps previous data for smooth pagination (`placeholderData: keepPreviousData`).
- **Debounced search**: 300 ms via `useDebouncedValue`.
- **Bundle**: Tailwind JIT purges unused classes; Vite emits split vendor chunks.
- Documented future upgrades: `React.memo` on `ProductTableRow` and `ImagePreviewTile`, virtualization at page size > 100.

---

## 6. Edge cases exercised

Both by the seed dataset and the Postman "Guided Demo Flow":

**Backend**

- Duplicate SKU on create → 409 `duplicate-key` (Postgres unique index violation surfaced by `DbUpdateException`).
- Stale `If-Match` → 409 with current server row version.
- Delete category still in use → 422 `Business Rule Violation`.
- Soft-deleted product queried by ID → 404 (global filter).
- Filter attribute not defined on any product → empty page 200 (not 404).
- Image upload wrong MIME / > 5 MB → rejected at endpoint before touching storage.
- Set a new image `IsPrimary = true` → aggregate demotes the previous primary.

**Frontend**

- API unreachable → TanStack Query retry + toast + retry button.
- Slow API → skeleton persists.
- Empty list → empty state component with primary CTA.
- Server validation error → highlighted field + toast summary.
- 409 concurrency → modal "Stay here / Reload".
- Delete → confirm dialog + optimistic remove + rollback on error.
- Deep link to soft-deleted product → 404 page.
- Non-Latin search characters → passed through untouched.
- ProductType change → attribute editor re-renders with new fields, preserving values that still apply.

---

## 7. Repository layout

```
.
├── server-app/                                Backend (Task 4)
│   ├── Bootstrapper/Api/                       Program.cs, appsettings, Dockerfile
│   ├── Modules/Catalog/Catalog/                Implemented module
│   ├── Shared/Shared/                          Cross-cutting building blocks
│   ├── docker-compose.yml                      Postgres 17 + Redis 7 + API
│   ├── fashion-shop.slnx
│   └── README.md
├── client-app/                                Frontend (Task 5)
│   ├── src/{app,features,shared}/
│   └── README.md
├── docs/
│   ├── report/report.md                       This document
│   ├── erd/README.md                          Mermaid ERD + generation notes
│   ├── postman/                               Collection + environment file
│   └── screenshots/                           Swagger + SPA screenshots
└── README.md
```

---

## 8. Limitations

1. **Auth is stubbed.** Audit user is hardcoded `"system"` in `AuditableEntityInterceptor`. Production wiring: JWT bearer + `ICurrentUser` + interceptor update.
2. **No automated tests** in the initial submission. High-value additions: `Product.Create` / `Product.SetAttribute` unit tests, `CreateProductCommandHandler` integration tests against Postgres testcontainers, Playwright e2e for the SPA golden path.
3. **In-memory cache invalidation is per-instance.** Redis is shared, but no keyspace notification pub/sub — a horizontally-scaled deployment needs `ProductChangedNotification` published to Redis pub/sub or a cache tag scheme.
4. **Image storage is local disk** (`wwwroot/uploads/products/`). Production needs S3-compatible object storage + CDN + signed URLs.
5. **No structured logging.** Serilog with a request-correlation middleware is a documented upgrade.
6. **No health check endpoint** — `AddHealthChecks().AddDbContextCheck<CatalogDbContext>().AddRedis(...)` + `MapHealthChecks("/health")` is a 10-line addition.
7. **Single-currency, single-locale** — no `ProductTranslation` or `Money` value object.
8. **Attribute values are all strings** in `product_attributes.value`. Analytics over numeric attributes will need casting or a JSONB mirror column.
9. **List queries are not cached** (only `GetById` is). A `CachingBehavior` pipeline with `ICacheableQuery` marker is the natural next step.
10. **No `AsSplitQuery()` on detail path** — safe today at seed scale, risky as images/attributes grow.
11. **Global exception handler does not yet return 428 for missing `If-Match`.** Today a missing `If-Match` bypasses the concurrency check — a strict deployment should return 428 Precondition Required.

---

## 9. Future improvements

- **Cache**: Redis pub/sub for cross-instance invalidation; `CachingBehavior<TRequest, TResponse>` for list queries with cache-tag invalidation.
- **Auth**: JWT + Keycloak or Auth0, role-based endpoint policies, refresh flow in the SPA.
- **Search**: `pg_trgm` GIN index on `products.name`; Postgres `tsvector` full-text; Meilisearch for typo-tolerance.
- **Media**: object storage (S3 / MinIO), background workers for thumbnails + WebP.
- **Event-driven integration**: outbox pattern so `Basket` and `Ordering` modules can react to `ProductPriceChanged`, `ProductDeactivated`.
- **Multi-tenant**: `TenantId` column, row-level security in Postgres.
- **Observability**: OpenTelemetry traces → Jaeger, metrics → Prometheus, Serilog → ELK.
- **CI**: GitHub Actions running `dotnet build`, `dotnet test`, `pnpm typecheck`, `pnpm lint`, `pnpm build`, and `dotnet ef migrations bundle` on every PR.
- **Frontend polish**: Playwright e2e, Storybook for shared UI, virtualized table, CSV import/export, image cropper, i18n (`en` / `vi`), PWA.
- **Rate limiting**: fixed-window 60/min per IP on write endpoints via `AddRateLimiter`.
- **Idempotency**: `Idempotency-Key` header on `POST /products` backed by a lightweight idempotency table.

---

## 10. Self-assessment against the rubric

| Criterion | Weight | Confidence | Evidence |
| --- | --- | --- | --- |
| Approach — dev process | 10 | High | Delivery spec + this report + phased commit history. |
| DB & project structure | 20 | High | EAV fully implemented, `xmin` concurrency, soft-delete global filter, modules isolated. |
| Tech stack (justified) | 10 | High | This report §3, per-app READMEs list every dependency + rationale. |
| API & data handling | 15 | Medium-High | 10 product endpoints, ETag round-trip, RFC 7807. Documented gaps: missing 428 on missing `If-Match`. |
| Performance | 10 | Medium | Distributed cache on hot read, `AsNoTracking` + projection on lists, lazy routes + code split on FE. Documented gaps: no list-cache, no `AsSplitQuery`, no `pg_trgm`. |
| Frontend delivery | 15 | High | Full CRUD flow, dynamic attribute editor, dropzone upload, 409 modal, dark mode, `keepPreviousData`. |
| Documentation | 5 | High | Root README + per-app READMEs + this report + ERD + Postman guided flow + screenshots. |

Known gaps are all documented in §8 / §9 and in the per-endpoint code comments where relevant.

---

## Appendix A — screenshots

See [../screenshots/](../screenshots/).

- Swagger UI overview.
- SPA product list page (light + dark mode).
- SPA product detail page.
- SPA product create form showing dynamic attribute editor.
- 409 concurrency modal in action.

## Appendix B — how to run

See [root README §1](../../README.md).

## Appendix C — AI-assisted Engineering Workflow

**Transparency note.** AI-assisted development tools were used as productivity aids during research, implementation, code review, and documentation. Final engineering decisions, implementation details, and validation remained the responsibility of the author. All architectural decisions, implementation changes, and final deliverables were manually reviewed and validated before submission.

| Activity | AI role | Author role |
| --- | --- | --- |
| Options analysis — e.g. Scrutor decorator vs. `CachingBehavior` pipeline, `xmin` vs. maintained `byte[]` RowVersion, EAV vs. per-type columns | Summarized alternatives and highlighted trade-offs. | Made the final call and defended it in this report |
| Boilerplate scaffolding — EF configurations, DTO records, Postman requests, README structure | Helped scaffold first drafts | Reviewed for correctness, adjusted to match codebase conventions |
| Documentation authoring — this report, root + per-app READMEs, ERD notes, Postman walkthrough | Helped organize and refine documentation. | Edited for accuracy and tone, removed anything not backed by the code |
| Code review & refactoring | Identified potential issues, proposed alternatives | Accepted / rejected each suggestion; every change tested locally |
| Behaviour verification — Docker boot, migrations, Swagger, SPA flows, ETag round-trip, 409 modal | — | Performed manually by the author |

The objective was to use AI as an engineering productivity tool while ensuring that all implementation decisions and delivered functionality remained fully understood, validated, and owned by the author.