# Fashion Shop Admin — Frontend

React 18 + TypeScript SPA for managing the Fashion Shop product catalog. Integrates with the [server-app](../server-app) .NET 8 API.

## Tech stack

| Concern | Choice |
| --- | --- |
| Build | Vite 5 |
| Framework | React 18 |
| Language | TypeScript 5.4 (strict) |
| Router | React Router 6 (lazy-loaded per route) |
| Server state | TanStack Query 5 |
| Client state | Zustand |
| HTTP client | axios |
| Forms | react-hook-form + Zod |
| UI | Tailwind CSS + Radix UI primitives |
| File upload | react-dropzone |
| Notifications | sonner |
| Tables | @tanstack/react-table |

## Prerequisites

- Node 20 LTS
- pnpm 9+ (`npm i -g pnpm`)
- The backend API running on `http://localhost:5005` (see [server-app README](../server-app/README.md))

## Quick start

```bash
cd client-app

# Copy env file and adjust if needed
cp .env.example .env.development

# Install dependencies
pnpm install

# Start dev server → http://localhost:5173
pnpm dev
```

## Environment variables

| Variable | Default | Description |
| --- | --- | --- |
| `VITE_API_BASE_URL` | `http://localhost:5005/api/v1` | Backend API base URL |
| `VITE_UPLOADS_BASE_URL` | `http://localhost:5005` | Base URL for resolving uploaded image paths |
| `VITE_APP_NAME` | `Fashion Shop Admin` | App title shown in top bar |

## Scripts

```bash
pnpm dev        # Start dev server
pnpm build      # Type-check + production build → dist/
pnpm preview    # Preview production build locally
pnpm typecheck  # tsc --noEmit
pnpm lint       # ESLint
pnpm format     # Prettier
```

## Project structure

```
src/
├── app/                    # Router, providers, QueryClient, App entry
├── features/
│   ├── products/           # Core feature: list, detail, create, edit
│   │   ├── api/            # TanStack Query hooks + axios calls
│   │   ├── components/     # ProductTable, ProductForm, FilterBar, AttributeEditor, ImageUploader
│   │   ├── pages/          # ProductListPage, ProductDetailPage, ProductCreatePage, ProductEditPage
│   │   ├── schemas.ts      # Zod validation schemas
│   │   └── types.ts        # TypeScript types
│   ├── categories/         # Category CRUD (list + modal)
│   ├── brands/             # Brand CRUD
│   ├── product-types/      # ProductType CRUD + attribute management
│   └── attribute-definitions/  # AttributeDefinition CRUD
└── shared/
    ├── api/                # httpClient, ETag store, Problem Details parser
    ├── components/         # Button, Input, Dialog, Card, Badge, Select, Skeleton, ...
    ├── hooks/              # useDebouncedValue
    ├── layout/             # AppShell, Sidebar, Topbar, PageHeader, ErrorBoundary
    ├── stores/             # Zustand: uiStore (theme, sidebar)
    └── utils/              # cn, format (money/date), slugify
```

## Features

- **Product list**: server-side filter (search, category, brand, status), sort, pagination with debounced search.
- **Product detail**: full attribute list, image gallery, audit timestamps.
- **Create product**: multi-section form with dynamic attribute editor (fields rendered from ProductType definition), auto-generated slug, Zod validation.
- **Edit product**: pre-filled form, optimistic concurrency via `ETag/If-Match` — 409 triggers a reload dialog.
- **Image upload**: react-dropzone, per-file progress bar, max 5 images, 5 MB/file, JPG/PNG/WebP.
- **Dark mode**: toggle via top bar, persisted in localStorage.
- **Error handling**: RFC 7807 Problem Details → toast + inline field errors.
- **Reference data**: Categories, Brands, Product Types, Attribute Definitions — each a table with modal create/edit.

## Code style conventions

- **Function declarations** for React components (`export function Foo() {}`).
- **Arrow functions** for hooks, utils, and callbacks.
- **Named exports** everywhere (no default exports).
- **`type`** keyword for all prop types (`type FooProps = { … }`).
- Path alias `@/` → `src/`.
- `cn()` helper (`clsx` + `tailwind-merge`) for conditional classNames.
- 2-space indent, single quotes, trailing commas (enforced via Prettier).

## Limitations

- No authentication — API assumed open. Future: JWT login page + axios `Authorization` interceptor.
- Images stored on local disk — production needs S3-compatible storage + CDN.
- No i18n — UI is English only. Future: react-i18next.
- No unit/e2e tests in the initial submission. Future: Vitest + Playwright.
- No offline support — TanStack Query cache is memory-only.

## Future improvements

- Auth flow (JWT / Keycloak).
- Playwright e2e tests for golden path.
- Virtualized table for very large product lists.
- CSV / Excel import-export.
- Image crop + compression before upload.
- i18n (en / vi).
- CI: GitHub Actions `typecheck` + `lint` + `build` on every PR.
