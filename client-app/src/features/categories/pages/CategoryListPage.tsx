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
  useCategoriesQuery,
  useCreateCategoryMutation,
  useDeleteCategoryMutation,
  useUpdateCategoryMutation,
} from '@/features/categories/api/categoryHooks';
import type { Category } from '@/features/categories/types';

const categorySchema = z.object({
  name: z.string().trim().min(1, 'Name is required').max(100),
  slug: z.string().trim().min(1, 'Slug is required').max(120).regex(/^[a-z0-9-]+$/, 'Lowercase kebab-case only'),
});

type CategoryFormValues = z.infer<typeof categorySchema>;

export function CategoryListPage() {
  const { data: categories, isLoading } = useCategoriesQuery();
  const createMutation = useCreateCategoryMutation();
  const updateMutation = useUpdateCategoryMutation();
  const deleteMutation = useDeleteCategoryMutation();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<Category | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    formState: { errors },
  } = useForm<CategoryFormValues>({
    resolver: zodResolver(categorySchema),
    defaultValues: { name: '', slug: '' },
  });

  const openCreate = () => {
    setEditing(null);
    reset({ name: '', slug: '' });
    setDialogOpen(true);
  };

  const openEdit = (category: Category) => {
    setEditing(category);
    reset({ name: category.name, slug: category.slug });
    setDialogOpen(true);
  };

  const onSubmit = handleSubmit((values) => {
    if (editing) {
      updateMutation.mutate(
        { id: editing.id, payload: values },
        { onSuccess: () => setDialogOpen(false) },
      );
    } else {
      createMutation.mutate(values, {
        onSuccess: () => setDialogOpen(false),
      });
    }
  });

  const isPending = createMutation.isPending || updateMutation.isPending;

  return (
    <>
      <ResourceCrudTemplate
        title="Categories"
        items={categories}
        isLoading={isLoading}
        isDeleting={deleteMutation.isPending}
        createLabel="New Category"
        onCreateClick={openCreate}
        onEditClick={openEdit}
        onDeleteConfirm={(item) => deleteMutation.mutate(item.id)}
        columns={[
          { header: 'Name', cell: (c) => <span className="font-medium">{c.name}</span> },
          { header: 'Slug', cell: (c) => <code className="text-xs">{c.slug}</code> },
        ]}
      />

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editing ? 'Edit Category' : 'New Category'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={onSubmit} className="space-y-4">
            <FormField id="cat-name" label="Name" error={errors.name?.message} required>
              <Input
                id="cat-name"
                {...register('name', {
                  onChange: (e) => {
                    if (!editing) setValue('slug', slugify(e.target.value));
                  },
                })}
                placeholder="e.g. T-Shirts"
              />
            </FormField>
            <FormField id="cat-slug" label="Slug" error={errors.slug?.message} required>
              <Input id="cat-slug" {...register('slug')} placeholder="t-shirts" />
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
