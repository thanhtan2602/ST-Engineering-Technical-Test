# Postman Collection

Complete API collection for the Fashion Shop backend, with an included environment file that pre-loads the seeded IDs so the collection is runnable end-to-end on a fresh Docker Compose boot.

## Files

- **[FashionShop.postman_collection.json](FashionShop.postman_collection.json)** — 66 requests grouped into 7 folders: `Categories`, `Brands`, `Attribute Definitions`, `Product Types`, `Products`, `_Guided Demo Flow`, plus error scenarios.
- **[FashionShop.postman_environment.json](FashionShop.postman_environment.json)** — `baseUrl`, seeded IDs (`categoryId`, `brandId`, `productTypeId`, `colorAttrId`, `sizeAttrId`, `productId`, ...) and auto-captured runtime IDs (`productEtag`, `newProductId`, ...).

## Import

1. Open Postman → **Import** → drop both JSON files.
2. In the top-right environment picker, select **FashionShop - Local**.
3. Boot the backend (`cd server-app && docker compose up --build`) — Swagger becomes available at http://localhost:5005/swagger.

## Base URL

The environment defaults to `http://localhost:5005/api/v1` (Docker Compose host port).
If you run the API with `dotnet run` outside Docker, the port is the same (`5005`) — `launchSettings.json` binds Kestrel to `http://localhost:5005` in both cases.

## Guided Demo Flow

The `_Guided Demo Flow` folder is the fastest way to convince yourself the whole system works. Run it top-to-bottom (right-click → **Run folder**). It covers:

1. `GET /product-types/{Fashion}` — verify seed loaded.
2. `GET /products` — see the seeded list.
3. `GET /products/{id}` (Adidas Stan Smith) — **captures the ETag** into `{{productEtag}}` via a test script.
4. `PUT /products/{id}` with the fresh ETag — 200 OK, new row version.
5. `PUT /products/{id}` with the **same now-stale ETag** — **409 Conflict** with the current server row version in the Problem Details body.
6. `PATCH /products/{id}/status → Inactive`.
7. `POST /products/{id}/attributes` — change `Color` to `Red`.
8. `POST /products` — create a new product; ID captured to `{{newProductId}}`.
9. `POST /products/{newProductId}/images` — multipart upload; image ID captured to `{{newImageId}}`.
10. `DELETE /products/{newProductId}` — soft-delete.
11. `GET /products/{newProductId}` → **404 Not Found** (global soft-delete filter hides the row).

## Error-contract requests

The `Products` folder also includes negative cases:

- `Create Product – Error: missing required attrs (400)` — omitted required attribute → 400 with `errors: { attributes: [msg] }`.
- `Create Product – Error: invalid enum value (400)` — attribute value not in `AllowedValues` → 400.
- `Create Product – Error: FluentValidation (400 with errors dict)` — invalid SKU regex, negative price.
- `Update Product – STALE ETag (409 Conflict)` — reused stale token → 409.
- `Update Product – no ETag (bypasses concurrency)` — kept in the collection to document current behavior; see [../report/report.md §8 item 11](../report/report.md) for the discussion of returning 428 instead.
- `Get Deleted Product – 404` — proves the global soft-delete filter.
- `Get Product – 404 (non-existent)` — random GUID → 404.
- `Delete Category in-use → 422` / `Delete Brand in-use → 422` / `Delete ProductType in-use → 422` — cross-aggregate business rule.

## Regenerating the seeded environment IDs

The seeded IDs in the environment file mirror [server-app/Modules/Catalog/Catalog/Data/Seed/InitialData.cs](../../server-app/Modules/Catalog/Catalog/Data/Seed/InitialData.cs). If the seed is edited, update the environment file to match — or run the `_Guided Demo Flow` to have Postman capture fresh IDs into the `new*` variables.
