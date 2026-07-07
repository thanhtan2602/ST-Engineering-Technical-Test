import { useEffect } from 'react';
import { useFieldArray, useFormContext } from 'react-hook-form';
import { Input } from '@/shared/components/Input';
import { Switch } from '@/shared/components/Switch';
import { FormField } from '@/shared/components/FormField';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/Select';
import { Badge } from '@/shared/components/Badge';
import type { ProductFormValues } from '@/features/products/schemas';
import type { ProductTypeAttribute } from '@/features/product-types/types';

type AttributeEditorProps = {
  typeAttributes: ProductTypeAttribute[];
};

export function AttributeEditor({ typeAttributes }: AttributeEditorProps) {
  const {
    control,
    register,
    setValue,
    watch,
    formState: { errors },
  } = useFormContext<ProductFormValues>();

  const { fields, replace } = useFieldArray({ control, name: 'attributes' });

  const attributeValues = watch('attributes');

  useEffect(() => {
    if (typeAttributes.length === 0) return;

    const currentIds = new Set((attributeValues ?? []).map((a) => a.attributeDefinitionId));
    const targetIds = typeAttributes.map((a) => a.attributeDefinitionId);
    const inSync =
      targetIds.length === currentIds.size &&
      targetIds.every((id) => currentIds.has(id));

    if (inSync) return;

    const preserved = (attributeValues ?? []).reduce<Record<string, string>>((acc, a) => {
      acc[a.attributeDefinitionId] = a.value;
      return acc;
    }, {});

    replace(
      typeAttributes.map((attr) => ({
        attributeDefinitionId: attr.attributeDefinitionId,
        value: preserved[attr.attributeDefinitionId] ?? '',
      })),
    );
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [typeAttributes, replace]);

  if (typeAttributes.length === 0) {
    return (
      <p className="text-sm text-muted-foreground">
        No attributes defined for this product type.
      </p>
    );
  }

  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
      {fields.map((field, index) => {
        const attrDef = typeAttributes.find((a) => a.attributeDefinitionId === field.attributeDefinitionId);
        if (!attrDef) return null;

        const error = errors.attributes?.[index]?.value?.message;
        const label = (
          <span className="flex items-center gap-2">
            {attrDef.label}
            {attrDef.isRequired && <Badge variant="secondary" className="text-[10px]">required</Badge>}
            {attrDef.unit && (
              <span className="text-xs text-muted-foreground">({attrDef.unit})</span>
            )}
          </span>
        );

        if (attrDef.dataType === 'Boolean') {
          return (
            <div key={field.id} className="flex items-center justify-between rounded-md border p-3">
              <span className="text-sm font-medium">{attrDef.label}</span>
              <Switch
                checked={attributeValues[index]?.value === 'true'}
                onCheckedChange={(checked) =>
                  setValue(`attributes.${index}.value`, String(checked), { shouldValidate: true })
                }
              />
            </div>
          );
        }

        if (attrDef.dataType === 'Enum' && attrDef.allowedValues?.length) {
          return (
            <FormField key={field.id} label={attrDef.label} error={error} required={attrDef.isRequired}>
              <Select
                value={attributeValues[index]?.value ?? ''}
                onValueChange={(v) =>
                  setValue(`attributes.${index}.value`, v, { shouldValidate: true })
                }
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select…" />
                </SelectTrigger>
                <SelectContent>
                  {attrDef.allowedValues.map((v) => (
                    <SelectItem key={v} value={v}>
                      {v}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </FormField>
          );
        }

        return (
          <FormField
            key={field.id}
            id={`attr-${field.id}`}
            label={typeof label === 'string' ? label : attrDef.label}
            error={error}
            required={attrDef.isRequired}
          >
            <Input
              id={`attr-${field.id}`}
              type={attrDef.dataType === 'Number' ? 'number' : attrDef.dataType === 'Date' ? 'date' : 'text'}
              {...register(`attributes.${index}.value`)}
              placeholder={attrDef.unit ? `e.g. 42 ${attrDef.unit}` : undefined}
            />
          </FormField>
        );
      })}
    </div>
  );
}
