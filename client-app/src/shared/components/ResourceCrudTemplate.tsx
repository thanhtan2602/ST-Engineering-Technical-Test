import { useState } from 'react';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { Button } from '@/shared/components/Button';
import { Skeleton } from '@/shared/components/Skeleton';
import { EmptyState } from '@/shared/components/EmptyState';
import { ConfirmDialog } from '@/shared/components/ConfirmDialog';
import { PageHeader } from '@/shared/layout/PageHeader';

export type ResourceColumn<T> = {
  header: string;
  cell: (item: T) => React.ReactNode;
};

type ResourceCrudTemplateProps<T extends { id: string }> = {
  title: string;
  description?: string;
  items: T[] | undefined;
  isLoading: boolean;
  columns: ResourceColumn<T>[];
  isDeleting?: boolean;
  createLabel?: string;
  onCreateClick: () => void;
  onEditClick: (item: T) => void;
  onDeleteConfirm: (item: T) => void;
  deleteTitle?: (item: T) => string;
  deleteDescription?: (item: T) => string;
};

export function ResourceCrudTemplate<T extends { id: string }>({
  title,
  description,
  items,
  isLoading,
  columns,
  isDeleting,
  createLabel = 'New',
  onCreateClick,
  onEditClick,
  onDeleteConfirm,
  deleteTitle,
  deleteDescription,
}: ResourceCrudTemplateProps<T>) {
  const [deleteTarget, setDeleteTarget] = useState<T | null>(null);

  return (
    <div>
      <PageHeader
        title={title}
        description={description ?? (items ? `${items.length} items` : undefined)}
        actions={
          <Button onClick={onCreateClick}>
            <Plus className="mr-2 h-4 w-4" />
            {createLabel}
          </Button>
        }
      />

      {isLoading ? (
        <div className="space-y-3">
          {Array.from({ length: 4 }).map((_, i) => (
            <Skeleton key={i} className="h-12 w-full" />
          ))}
        </div>
      ) : !items || items.length === 0 ? (
        <EmptyState
          title={`No ${title.toLowerCase()} yet`}
          description={`Click "${createLabel}" to add the first one.`}
          action={
            <Button onClick={onCreateClick}>
              <Plus className="mr-2 h-4 w-4" />
              {createLabel}
            </Button>
          }
        />
      ) : (
        <div className="overflow-x-auto rounded-md border">
          <table className="w-full text-sm">
            <thead className="border-b bg-muted/50">
              <tr>
                {columns.map((col) => (
                  <th
                    key={col.header}
                    className="px-4 py-3 text-left text-xs font-medium text-muted-foreground"
                  >
                    {col.header}
                  </th>
                ))}
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map((item) => (
                <tr key={item.id} className="hover:bg-muted/30 transition-colors">
                  {columns.map((col) => (
                    <td key={col.header} className="px-4 py-3">
                      {col.cell(item)}
                    </td>
                  ))}
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-end gap-1">
                      <Button
                        variant="ghost"
                        size="icon"
                        onClick={() => onEditClick(item)}
                        title="Edit"
                      >
                        <Pencil className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="text-destructive hover:text-destructive"
                        onClick={() => setDeleteTarget(item)}
                        title="Delete"
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <ConfirmDialog
        open={!!deleteTarget}
        title={deleteTarget && deleteTitle ? deleteTitle(deleteTarget) : 'Delete item'}
        description={
          deleteTarget && deleteDescription
            ? deleteDescription(deleteTarget)
            : deleteTarget
              ? `Are you sure you want to delete this item?`
              : undefined
        }
        confirmLabel="Delete"
        variant="destructive"
        loading={isDeleting}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null); }}
        onConfirm={() => {
          if (!deleteTarget) return;
          onDeleteConfirm(deleteTarget);
          setDeleteTarget(null);
        }}
      />
    </div>
  );
}
