import { createBrowserRouter, Navigate } from 'react-router-dom';
import { AppShell } from '@/shared/layout/AppShell';
import { ErrorBoundary } from '@/shared/layout/ErrorBoundary';
import { NotFoundPage } from '@/shared/layout/NotFoundPage';

export const router: ReturnType<typeof createBrowserRouter> = createBrowserRouter([
  {
    element: <AppShell />,
    errorElement: <ErrorBoundary />,
    children: [
      { index: true, element: <Navigate to="/products" replace /> },
      {
        path: 'products',
        lazy: async () => {
          const { ProductListPage } = await import('@/features/products/pages/ProductListPage');
          return { Component: ProductListPage };
        },
      },
      {
        path: 'products/new',
        lazy: async () => {
          const { ProductCreatePage } = await import('@/features/products/pages/ProductCreatePage');
          return { Component: ProductCreatePage };
        },
      },
      {
        path: 'products/:id',
        lazy: async () => {
          const { ProductDetailPage } = await import('@/features/products/pages/ProductDetailPage');
          return { Component: ProductDetailPage };
        },
      },
      {
        path: 'products/:id/edit',
        lazy: async () => {
          const { ProductEditPage } = await import('@/features/products/pages/ProductEditPage');
          return { Component: ProductEditPage };
        },
      },
      {
        path: 'categories',
        lazy: async () => {
          const { CategoryListPage } = await import('@/features/categories/pages/CategoryListPage');
          return { Component: CategoryListPage };
        },
      },
      {
        path: 'brands',
        lazy: async () => {
          const { BrandListPage } = await import('@/features/brands/pages/BrandListPage');
          return { Component: BrandListPage };
        },
      },
      {
        path: 'product-types',
        lazy: async () => {
          const { ProductTypeListPage } = await import(
            '@/features/product-types/pages/ProductTypeListPage'
          );
          return { Component: ProductTypeListPage };
        },
      },
      {
        path: 'attribute-definitions',
        lazy: async () => {
          const { AttributeDefinitionListPage } = await import(
            '@/features/attribute-definitions/pages/AttributeDefinitionListPage'
          );
          return { Component: AttributeDefinitionListPage };
        },
      },
      { path: '*', element: <NotFoundPage /> },
    ],
  },
]);
