import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Badge } from '@/shared/components/Badge';
import { ResourceCrudTemplate } from '@/shared/components/ResourceCrudTemplate';
import { Button } from '@/shared/components/Button';
import { Input } from '@/shared/components/Input';
import { FormField } from '@/shared/components/FormField';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/Select';
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/Dialog';
import {
  useAttributeDefinitionsQuery,
  useCreateAttributeDefinitionMutation,
  useDeleteAttributeDefinitionMutation,
  useUpdateAttributeDefinitionMutation,
} from '@/features/attribute-definitions/api/attributeDefinitionHooks';
import type { AttributeDefinition } from '@/features/attribute-definitions/types';
import type { AttributeDataType } from '@/features/products/types';

const DATA_TYPES: AttributeDataType[] = ['Text', 'Number', 'Boolean', 'Date', 'Enum'];

const attrSchema = z.object({
  key: z
    .string()
    .trim()
    .min(1, 'Key is required')
    .max(64)
    .regex(/^[a-z_]+$/, 'Lowercase with underscores only'),
  label: z.string().trim().min(1, 'Label is required').max(100),
  dataType: z.enum(['Text', 'Number', 'Boolean', 'Date', 'Enum']),
  unit: z.string().max(20).optional().or(z.literal('')),
  allowedValues: z.string().optional(),
});

type AttrFormValues = z.infer<typeof attrSchema>;

export function AttributeDefinitionListPage() {
  const { data: attributes, isLoading } = useAttributeDefinitionsQuery();
  const createMutation = useCreateAttributeDefinitionMutation();
  const updateMutation = useUpdateAttributeDefinitionMutation();
  const deleteMutation = useDeleteAttributeDefinitionMutation();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<AttributeDefinition | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    watch,
    setValue,
    formState: { errors },
  } = useForm<AttrFormValues>({
    resolver: zodResolver(attrSchema),
    defaultValues: { key: '', label: '', dataType: 'Text', unit: '', allowedValues: '' },
  });

  const dataType = watch('dataType');

  const openCreate = () => {
    setEditing(null);
    reset({ key: '', label: '', dataType: 'Text', unit: '', allowedValues: '' });
    setDialogOpen(true);
  };

  const openEdit = (attr: AttributeDefinition) => {
    setEditing(attr);
    reset({
      key: attr.key,
      label: attr.label,
      dataType: attr.dataType,
      unit: attr.unit ?? '',
      allowedValues: attr.allowedValues?.join(', ') ?? '',
    });
    setDialogOpen(true);
  };

  const onSubmit = handleSubmit((values) => {
    const payload = {
      key: values.key,
      label: values.label,
      dataType: values.dataType,
      unit: values.unit || undefined,
      allowedValues:
        values.dataType === 'Enum' && values.allowedValues
          ? values.allowedValues
              .split(',')
              .map((v) => v.trim())
              .filter(Boolean)
          : undefined,
    };

    if (editing) {
      updateMutation.mutate(
        { id: editing.id, payload },
        { onSuccess: () => setDialogOpen(false) },
      );
    } else {
      createMutation.mutate(payload, { onSuccess: () => setDialogOpen(false) });
    }
  });

  const isPending = createMutation.isPending || updateMutation.isPending;

  return (
    <>
      <ResourceCrudTemplate
        title="Attribute Definitions"
        items={attributes}
        isLoading={isLoading}
        isDeleting={deleteMutation.isPending}
        createLabel="New Attribute"
        onCreateClick={openCreate}
        onEditClick={openEdit}
        onDeleteConfirm={(item) => deleteMutation.mutate(item.id)}
        columns={[
          {
            header: 'Key',
            cell: (a) => <code className="text-xs font-semibold">{a.key}</code>,
          },
          { header: 'Label', cell: (a) => a.label },
          {
            header: 'Type',
            cell: (a) => <Badge variant="secondary">{a.dataType}</Badge>,
          },
          {
            header: 'Unit / Values',
            cell: (a) =>
              a.dataType === 'Enum' && a.allowedValues?.length ? (
                <span className="text-xs text-muted-foreground">
                  {a.allowedValues.join(', ')}
                </span>
              ) : a.unit ? (
                <span className="text-xs text-muted-foreground">{a.unit}</span>
              ) : (
                <span className="text-xs text-muted-foreground">—</span>
              ),
          },
        ]}
      />

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editing ? 'Edit Attribute' : 'New Attribute'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={onSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <FormField id="attr-key" label="Key" error={errors.key?.message} required hint="e.g. color">
                <Input id="attr-key" {...register('key')} placeholder="color" disabled={!!editing} />
              </FormField>
              <FormField id="attr-label" label="Display Label" error={errors.label?.message} required>
                <Input id="attr-label" {...register('label')} placeholder="Color" />
              </FormField>
            </div>

            <FormField id="attr-datatype" label="Data Type" error={errors.dataType?.message} required>
              <Select
                value={dataType}
                onValueChange={(v) => setValue('dataType', v as AttributeDataType)}
              >
                <SelectTrigger id="attr-datatype">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {DATA_TYPES.map((t) => (
                    <SelectItem key={t} value={t}>
                      {t}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </FormField>

            {dataType !== 'Boolean' && dataType !== 'Enum' && (
              <FormField id="attr-unit" label="Unit (optional)" error={errors.unit?.message} hint="e.g. kg, cm">
                <Input id="attr-unit" {...register('unit')} placeholder="kg" />
              </FormField>
            )}

            {dataType === 'Enum' && (
              <FormField
                id="attr-allowed"
                label="Allowed Values"
                error={errors.allowedValues?.message}
                hint="Comma-separated list, e.g. Red, Green, Blue"
                required
              >
                <Input id="attr-allowed" {...register('allowedValues')} placeholder="Red, Green, Blue" />
              </FormField>
            )}

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
