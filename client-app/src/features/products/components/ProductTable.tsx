import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Pencil, Trash2, Eye } from 'lucide-react';
import {
  createColumnHelper,
  flexRender,
  getCoreRowModel,
  useReactTable,
} from '@tanstack/react-table';
import { Button } from '@/shared/components/Button';
import { Skeleton } from '@/shared/components/Skeleton';
import { EmptyState } from '@/shared/components/EmptyState';
import { ConfirmDialog } from '@/shared/components/ConfirmDialog';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/Select';
import { ProductStatusBadge } from '@/features/products/components/ProductStatusBadge';
import { useDeleteProductMutation } from '@/features/products/api/useProductMutations';
import { resolveUploadUrl } from '@/shared/api/httpClient';
import { formatMoney } from '@/shared/utils/format';
import type { PagedResult } from '@/shared/api/types';
import type { ProductListFilters, ProductListItem } from '@/features/products/types';

type ProductTableProps = {
  data: PagedResult<ProductListItem> | undefined;
  isLoading: boolean;
  filters: ProductListFilters;
  onFiltersChange: (patch: Partial<ProductListFilters>) => void;
};

const col = createColumnHelper<ProductListItem>();

export function ProductTable({ data, isLoading, filters, onFiltersChange }: ProductTableProps) {
  const navigate = useNavigate();
  const deleteMutation = useDeleteProductMutation();
  const [deleteTarget, setDeleteTarget] = useState<ProductListItem | null>(null);

  const columns = [
    col.display({
      id: 'image',
      header: '',
      size: 56,
      cell: ({ row }) => {
        const url = row.original.primaryImageUrl
          ? resolveUploadUrl(row.original.primaryImageUrl)
          : null;
        return url ? (
          <img
            src={url}
            alt={row.original.name}
            className="h-10 w-10 rounded-md object-cover"
          />
        ) : (
          <div className="flex h-10 w-10 items-center justify-center rounded-md bg-muted text-xs text-muted-foreground">
            —
          </div>
        );
      },
    }),
    col.accessor('sku', {
      header: 'SKU',
      cell: (info) => <span className="font-mono text-xs">{info.getValue()}</span>,
    }),
    col.accessor('name', {
      header: 'Name',
      cell: (info) => (
        <Link
          to={`/products/${info.row.original.id}`}
          className="font-medium hover:underline"
        >
          {info.getValue()}
        </Link>
      ),
    }),
    col.accessor('categoryName', { header: 'Category' }),
    col.accessor('brandName', { header: 'Brand' }),
    col.accessor('price', {
      header: 'Price',
      cell: (info) => <span className="tabular-nums">{formatMoney(info.getValue())}</span>,
    }),
    col.accessor('status', {
      header: 'Status',
      cell: (info) => <ProductStatusBadge status={info.getValue()} />,
    }),
    col.display({
      id: 'actions',
      header: '',
      size: 48,
      cell: ({ row }) => (
        <div className="flex items-center justify-end gap-1">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => navigate(`/products/${row.original.id}`)}
            title="View"
          >
            <Eye className="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="icon"
            onClick={() => navigate(`/products/${row.original.id}/edit`)}
            title="Edit"
          >
            <Pencil className="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="icon"
            className="text-destructive hover:text-destructive"
            onClick={() => setDeleteTarget(row.original)}
            title="Delete"
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      ),
    }),
  ];

  const table = useReactTable({
    data: data?.data ?? [],
    columns,
    getCoreRowModel: getCoreRowModel(),
    manualPagination: true,
    pageCount: data?.totalPages ?? 0,
  });

  if (isLoading) {
    return (
      <div className="space-y-3">
        {Array.from({ length: 5 }).map((_, i) => (
          <Skeleton key={i} className="h-12 w-full" />
        ))}
      </div>
    );
  }

  if (!isLoading && (data?.data.length ?? 0) === 0) {
    return (
      <EmptyState
        title="No products found"
        description="Try adjusting your filters, or create a new product to get started."
      />
    );
  }

  return (
    <>
      <div className="overflow-x-auto rounded-md border">
        <table className="w-full text-sm">
          <thead className="border-b bg-muted/50">
            {table.getHeaderGroups().map((hg) => (
              <tr key={hg.id}>
                {hg.headers.map((header) => (
                  <th
                    key={header.id}
                    className="px-4 py-3 text-left text-xs font-medium text-muted-foreground"
                    style={{ width: header.getSize() !== 150 ? header.getSize() : undefined }}
                  >
                    {flexRender(header.column.columnDef.header, header.getContext())}
                  </th>
                ))}
              </tr>
            ))}
          </thead>
          <tbody className="divide-y">
            {table.getRowModel().rows.map((row) => (
              <tr key={row.id} className="hover:bg-muted/30 transition-colors">
                {row.getVisibleCells().map((cell) => (
                  <td key={cell.id} className="px-4 py-3">
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Footer: page size + pagination */}
      <div className="flex items-center justify-between pt-3 text-sm">
        <div className="flex items-center gap-2 text-muted-foreground">
          <span>Rows per page</span>
          <Select
            value={String(filters.pageSize)}
            onValueChange={(v) => onFiltersChange({ pageSize: Number(v), page: 1 })}
          >
            <SelectTrigger className="h-8 w-[70px]">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              {[10, 20, 50].map((n) => (
                <SelectItem key={n} value={String(n)}>
                  {n}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div className="flex items-center gap-2">
          <span className="text-muted-foreground">
            {data ? `${(filters.page - 1) * filters.pageSize + 1}–${Math.min(filters.page * filters.pageSize, data.totalItems)} / ${data.totalItems}` : ''}
          </span>
          <Button
            variant="outline"
            size="sm"
            disabled={filters.page <= 1}
            onClick={() => onFiltersChange({ page: filters.page - 1 })}
          >
            Previous
          </Button>
          <Button
            variant="outline"
            size="sm"
            disabled={!data || filters.page >= data.totalPages}
            onClick={() => onFiltersChange({ page: filters.page + 1 })}
          >
            Next
          </Button>
        </div>
      </div>

      <ConfirmDialog
        open={!!deleteTarget}
        title="Delete product"
        description={`Are you sure you want to delete "${deleteTarget?.name}"? This action cannot be undone.`}
        confirmLabel="Delete"
        variant="destructive"
        loading={deleteMutation.isPending}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        onConfirm={() => {
          if (!deleteTarget) return;
          deleteMutation.mutate(deleteTarget.id, {
            onSuccess: () => setDeleteTarget(null),
          });
        }}
      />
    </>
  );
}
