import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { ResourceCrudTemplate } from '@/shared/components/ResourceCrudTemplate';
import { Button } from '@/shared/components/Button';
import { Input } from '@/shared/components/Input';
import { FormField } from '@/shared/components/FormField';
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/Dialog';
import {
  useProductTypesQuery,
  useCreateProductTypeMutation,
  useDeleteProductTypeMutation,
  useUpdateProductTypeMutation,
} from '@/features/product-types/api/productTypeHooks';
import type { ProductTypeSummary } from '@/features/product-types/types';

const productTypeSchema = z.object({
  name: z.string().trim().min(1, 'Name is required').max(100),
  code: z.string().trim().min(1, 'Code is required').max(120).regex(/^[a-z0-9-]+$/, 'Lowercase kebab-case only'),
});

type ProductTypeFormValues = z.infer<typeof productTypeSchema>;

export function ProductTypeListPage() {
  const { data: productTypes, isLoading } = useProductTypesQuery();
  const createMutation = useCreateProductTypeMutation();
  const updateMutation = useUpdateProductTypeMutation();
  const deleteMutation = useDeleteProductTypeMutation();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<ProductTypeSummary | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    formState: { errors },
  } = useForm<ProductTypeFormValues>({
    resolver: zodResolver(productTypeSchema),
    defaultValues: { name: '', code: '' },
  });

  const openCreate = () => {
    setEditing(null);
    reset({ name: '', code: '' });
    setDialogOpen(true);
  };

  const openEdit = (pt: ProductTypeSummary) => {
    setEditing(pt);
    reset({ name: pt.name, code: pt.code });
    setDialogOpen(true);
  };

  const onSubmit = handleSubmit((values) => {
    if (editing) {
      updateMutation.mutate(
        { id: editing.id, payload: { name: values.name, code: values.code } },
        { onSuccess: () => setDialogOpen(false) },
      );
    } else {
      createMutation.mutate(
        { name: values.name, code: values.code },
        { onSuccess: () => setDialogOpen(false) },
      );
    }
  });

  const isPending = createMutation.isPending || updateMutation.isPending;

  return (
    <>
      <ResourceCrudTemplate
        title="Product Types"
        items={productTypes}
        isLoading={isLoading}
        isDeleting={deleteMutation.isPending}
        createLabel="New Type"
        onCreateClick={openCreate}
        onEditClick={openEdit}
        onDeleteConfirm={(item) => deleteMutation.mutate(item.id)}
        deleteDescription={(item) => `Are you sure you want to delete "${item.name}"?`}
        columns={[
          { header: 'Name', cell: (pt) => <span className="font-medium">{pt.name}</span> },
          { header: 'Code', cell: (pt) => <code className="text-xs">{pt.code}</code> },
        ]}
      />

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editing ? 'Edit Product Type' : 'New Product Type'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={onSubmit} className="space-y-4">
            <FormField id="pt-name" label="Name" error={errors.name?.message} required>
              <Input
                id="pt-name"
                {...register('name', {
                  onChange: (e) => {
                    if (!editing) setValue('code', e.target.value.toLowerCase().replace(/\s+/g, '-').replace(/[^a-z0-9-]/g, ''));
                  },
                })}
                placeholder="e.g. T-Shirt"
              />
            </FormField>
            <FormField id="pt-code" label="Code" error={errors.code?.message} required hint="Lowercase kebab-case">
              <Input id="pt-code" {...register('code')} placeholder="t-shirt" disabled={!!editing} />
            </FormField>
            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => setDialogOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={isPending}>
                {isPending ? 'Saving…' : editing ? 'Save' : 'Create'}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>
    </>
  );
}
