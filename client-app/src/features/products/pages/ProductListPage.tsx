import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Plus } from 'lucide-react';
import { Button } from '@/shared/components/Button';
import { PageHeader } from '@/shared/layout/PageHeader';
import { ProductFilterBar } from '@/features/products/components/ProductFilterBar';
import { ProductTable } from '@/features/products/components/ProductTable';
import { useProductsQuery } from '@/features/products/api/useProductsQuery';
import { useDebouncedValue } from '@/shared/hooks/useDebouncedValue';
import type { ProductListFilters } from '@/features/products/types';

const DEFAULT_FILTERS: ProductListFilters = {
  page: 1,
  pageSize: 20,
  sort: 'Newest',
};

export function ProductListPage() {
  const [filters, setFilters] = useState<ProductListFilters>(DEFAULT_FILTERS);
  const debouncedSearch = useDebouncedValue(filters.search, 300);

  const effectiveFilters: ProductListFilters = { ...filters, search: debouncedSearch };

  const { data, isLoading } = useProductsQuery(effectiveFilters);

  const handleFiltersChange = (patch: Partial<ProductListFilters>) => {
    setFilters((prev) => ({ ...prev, ...patch }));
  };

  return (
    <div>
      <PageHeader
        title="Products"
        description={data ? `${data.totalItems} products total` : undefined}
        actions={
          <Button asChild>
            <Link to="/products/new">
              <Plus className="mr-2 h-4 w-4" />
              New product
            </Link>
          </Button>
        }
      />
      <div className="space-y-4">
        <ProductFilterBar filters={filters} onChange={handleFiltersChange} />
        <ProductTable
          data={data}
          isLoading={isLoading}
          filters={filters}
          onFiltersChange={handleFiltersChange}
        />
      </div>
    </div>
  );
}
