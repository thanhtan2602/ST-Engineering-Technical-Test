import { Search, X } from 'lucide-react';
import { Input } from '@/shared/components/Input';
import { Button } from '@/shared/components/Button';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/Select';
import { useCategoriesQuery } from '@/features/categories/api/categoryHooks';
import { useBrandsQuery } from '@/features/brands/api/brandHooks';
import type { ProductListFilters, ProductSortOrder, ProductStatus } from '@/features/products/types';

type ProductFilterBarProps = {
  filters: ProductListFilters;
  onChange: (patch: Partial<ProductListFilters>) => void;
};

// Values match BE ProductStatus enum (JsonStringEnumConverter)
const STATUSES: Array<{ value: ProductStatus; label: string }> = [
  { value: 'Active',   label: 'Active'   },
  { value: 'Draft',    label: 'Draft'    },
  { value: 'Inactive', label: 'Inactive' },
];

// Values match BE ProductSortOrder enum (JsonStringEnumConverter)
const SORTS: Array<{ value: ProductSortOrder; label: string }> = [
  { value: 'Newest',    label: 'Newest'            },
  { value: 'PriceAsc',  label: 'Price: Low → High' },
  { value: 'PriceDesc', label: 'Price: High → Low' },
  { value: 'NameAsc',   label: 'Name A–Z'          },
  { value: 'NameDesc',  label: 'Name Z–A'          },
];

export function ProductFilterBar({ filters, onChange }: ProductFilterBarProps) {
  const { data: categories = [] } = useCategoriesQuery();
  const { data: brands = [] } = useBrandsQuery();

  const hasActiveFilter =
    !!filters.search ||
    !!filters.categoryId ||
    !!filters.brandId ||
    !!filters.status;

  const handleReset = () =>
    onChange({ search: '', categoryId: undefined, brandId: undefined, status: undefined, page: 1 });

  return (
    <div className="flex flex-wrap items-center gap-3">
      <div className="relative min-w-[200px] flex-1">
        <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
        <Input
          placeholder="Search products…"
          className="pl-9"
          value={filters.search ?? ''}
          onChange={(e) => onChange({ search: e.target.value, page: 1 })}
        />
      </div>

      <Select
        value={filters.categoryId ?? ''}
        onValueChange={(v) => onChange({ categoryId: v || undefined, page: 1 })}
      >
        <SelectTrigger className="w-[160px]">
          <SelectValue placeholder="Category" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="">All categories</SelectItem>
          {categories.map((c) => (
            <SelectItem key={c.id} value={c.id}>{c.name}</SelectItem>
          ))}
        </SelectContent>
      </Select>

      <Select
        value={filters.brandId ?? ''}
        onValueChange={(v) => onChange({ brandId: v || undefined, page: 1 })}
      >
        <SelectTrigger className="w-[140px]">
          <SelectValue placeholder="Brand" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="">All brands</SelectItem>
          {brands.map((b) => (
            <SelectItem key={b.id} value={b.id}>{b.name}</SelectItem>
          ))}
        </SelectContent>
      </Select>

      <Select
        value={filters.status ?? ''}
        onValueChange={(v) => onChange({ status: (v as ProductStatus) || undefined, page: 1 })}
      >
        <SelectTrigger className="w-[130px]">
          <SelectValue placeholder="Status" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="">All statuses</SelectItem>
          {STATUSES.map((s) => (
            <SelectItem key={s.value} value={s.value}>{s.label}</SelectItem>
          ))}
        </SelectContent>
      </Select>

      <Select
        value={filters.sort ?? 'Newest'}
        onValueChange={(v) => onChange({ sort: v as ProductSortOrder, page: 1 })}
      >
        <SelectTrigger className="w-[180px]">
          <SelectValue placeholder="Sort by" />
        </SelectTrigger>
        <SelectContent>
          {SORTS.map((s) => (
            <SelectItem key={s.value} value={s.value}>{s.label}</SelectItem>
          ))}
        </SelectContent>
      </Select>

      {hasActiveFilter && (
        <Button variant="ghost" size="sm" onClick={handleReset}>
          <X className="mr-1 h-4 w-4" />
          Clear
        </Button>
      )}
    </div>
  );
}
