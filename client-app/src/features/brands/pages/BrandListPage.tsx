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
import { slugify } from '@/shared/utils/format';
import {
  useBrandsQuery,
  useCreateBrandMutation,
  useDeleteBrandMutation,
  useUpdateBrandMutation,
} from '@/features/brands/api/brandHooks';
import type { Brand } from '@/features/brands/types';

const brandSchema = z.object({
  name: z.string().trim().min(1, 'Name is required').max(100),
  slug: z.string().trim().min(1, 'Slug is required').max(120).regex(/^[a-z0-9-]+$/, 'Lowercase kebab-case only'),
});

type BrandFormValues = z.infer<typeof brandSchema>;

export function BrandListPage() {
  const { data: brands, isLoading } = useBrandsQuery();
  const createMutation = useCreateBrandMutation();
  const updateMutation = useUpdateBrandMutation();
  const deleteMutation = useDeleteBrandMutation();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<Brand | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    formState: { errors },
  } = useForm<BrandFormValues>({
    resolver: zodResolver(brandSchema),
    defaultValues: { name: '', slug: '' },
  });

  const openCreate = () => {
    setEditing(null);
    reset({ name: '', slug: '' });
    setDialogOpen(true);
  };

  const openEdit = (brand: Brand) => {
    setEditing(brand);
    reset({ name: brand.name, slug: brand.slug });
    setDialogOpen(true);
  };

  const onSubmit = handleSubmit((values) => {
    if (editing) {
      updateMutation.mutate(
        { id: editing.id, payload: values },
        { onSuccess: () => setDialogOpen(false) },
      );
    } else {
      createMutation.mutate(values, { onSuccess: () => setDialogOpen(false) });
    }
  });

  const isPending = createMutation.isPending || updateMutation.isPending;

  return (
    <>
      <ResourceCrudTemplate
        title="Brands"
        items={brands}
        isLoading={isLoading}
        isDeleting={deleteMutation.isPending}
        createLabel="New Brand"
        onCreateClick={openCreate}
        onEditClick={openEdit}
        onDeleteConfirm={(item) => deleteMutation.mutate(item.id)}
        columns={[
          { header: 'Name', cell: (b) => <span className="font-medium">{b.name}</span> },
          { header: 'Slug', cell: (b) => <code className="text-xs">{b.slug}</code> },
        ]}
      />

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editing ? 'Edit Brand' : 'New Brand'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={onSubmit} className="space-y-4">
            <FormField id="brand-name" label="Name" error={errors.name?.message} required>
              <Input
                id="brand-name"
                {...register('name', {
                  onChange: (e) => {
                    if (!editing) setValue('slug', slugify(e.target.value));
                  },
                })}
                placeholder="e.g. Uniqlo"
              />
            </FormField>
            <FormField id="brand-slug" label="Slug" error={errors.slug?.message} required>
              <Input id="brand-slug" {...register('slug')} placeholder="uniqlo" />
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
