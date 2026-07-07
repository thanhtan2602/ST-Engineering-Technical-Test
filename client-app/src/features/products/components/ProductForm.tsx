import { useEffect } from 'react';
import { useFormContext } from 'react-hook-form';
import { Input } from '@/shared/components/Input';
import { Textarea } from '@/shared/components/Textarea';
import { FormField } from '@/shared/components/FormField';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/Card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/Select';
import { AttributeEditor } from '@/features/products/components/AttributeEditor';
import { useCategoriesQuery } from '@/features/categories/api/categoryHooks';
import { useBrandsQuery } from '@/features/brands/api/brandHooks';
import { useProductTypesQuery, useProductTypeQuery } from '@/features/product-types/api/productTypeHooks';
import { slugify } from '@/shared/utils/format';
import type { ProductFormValues } from '@/features/products/schemas';

type ProductFormProps = {
  isEdit?: boolean;
};

const STATUSES = [
  { value: 'Draft', label: 'Draft' },
  { value: 'Active', label: 'Active' },
  { value: 'Inactive', label: 'Inactive' },
] as const;

export function ProductForm({ isEdit = false }: ProductFormProps) {
  const {
    register,
    setValue,
    watch,
    formState: { errors },
  } = useFormContext<ProductFormValues>();

  const { data: categories = [] } = useCategoriesQuery();
  const { data: brands = [] } = useBrandsQuery();
  const { data: productTypes = [] } = useProductTypesQuery();

  const nameValue = watch('name');
  const productTypeId = watch('productTypeId');

  const { data: selectedProductType } = useProductTypeQuery(productTypeId || undefined);

  useEffect(() => {
    if (!isEdit && nameValue) {
      setValue('slug', slugify(nameValue), { shouldValidate: false });
    }
  }, [nameValue, isEdit, setValue]);

  return (
    <div className="space-y-6">
      {/* Basic Info */}
      <Card>
        <CardHeader>
          <CardTitle>Basic Information</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          {!isEdit && (
            <FormField id="sku" label="SKU" error={errors.sku?.message} required>
              <Input
                id="sku"
                {...register('sku')}
                placeholder="e.g. TSHIRT-BLK-M"
                className="uppercase"
              />
            </FormField>
          )}

          <FormField id="name" label="Name" error={errors.name?.message} required className={isEdit ? 'sm:col-span-2' : ''}>
            <Input id="name" {...register('name')} placeholder="Product name" />
          </FormField>

          <FormField
            id="slug"
            label="Slug"
            error={errors.slug?.message}
            hint="Lowercase kebab-case. Auto-generated from name."
            required
          >
            <Input id="slug" {...register('slug')} placeholder="product-slug" />
          </FormField>

          <FormField id="status" label="Status" error={errors.status?.message} required>
            <Select
              value={watch('status')}
              onValueChange={(v) => setValue('status', v as ProductFormValues['status'], { shouldValidate: true })}
            >
              <SelectTrigger id="status">
                <SelectValue placeholder="Select status" />
              </SelectTrigger>
              <SelectContent>
                {STATUSES.map((s) => (
                  <SelectItem key={s.value} value={s.value}>
                    {s.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </FormField>

          <FormField
            id="price"
            label="Price (VND)"
            error={errors.price?.message}
            required
          >
            <Input id="price" type="number" min={0} {...register('price')} placeholder="199000" />
          </FormField>

          <FormField id="categoryId" label="Category" error={errors.categoryId?.message} required>
            <Select
              value={watch('categoryId') ?? ''}
              onValueChange={(v) => setValue('categoryId', v, { shouldValidate: true })}
            >
              <SelectTrigger id="categoryId">
                <SelectValue placeholder="Select category" />
              </SelectTrigger>
              <SelectContent>
                {categories.map((c) => (
                  <SelectItem key={c.id} value={c.id}>
                    {c.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </FormField>

          <FormField id="brandId" label="Brand" error={errors.brandId?.message} required>
            <Select
              value={watch('brandId') ?? ''}
              onValueChange={(v) => setValue('brandId', v, { shouldValidate: true })}
            >
              <SelectTrigger id="brandId">
                <SelectValue placeholder="Select brand" />
              </SelectTrigger>
              <SelectContent>
                {brands.map((b) => (
                  <SelectItem key={b.id} value={b.id}>
                    {b.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </FormField>

          {!isEdit && (
            <FormField id="productTypeId" label="Product Type" error={errors.productTypeId?.message} required>
              <Select
                value={watch('productTypeId') ?? ''}
                onValueChange={(v) => setValue('productTypeId', v, { shouldValidate: true })}
              >
                <SelectTrigger id="productTypeId">
                  <SelectValue placeholder="Select product type" />
                </SelectTrigger>
                <SelectContent>
                  {productTypes.map((pt) => (
                    <SelectItem key={pt.id} value={pt.id}>
                      {pt.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </FormField>
          )}

          <FormField
            id="description"
            label="Description"
            error={errors.description?.message}
            className="sm:col-span-2"
          >
            <Textarea
              id="description"
              {...register('description')}
              placeholder="Optional product description…"
              className="min-h-[100px]"
            />
          </FormField>
        </CardContent>
      </Card>

      {/* Dynamic Attributes */}
      <Card>
        <CardHeader>
          <CardTitle>Attributes</CardTitle>
        </CardHeader>
        <CardContent>
          {productTypeId ? (
            <AttributeEditor typeAttributes={selectedProductType?.attributes ?? []} />
          ) : (
            <p className="text-sm text-muted-foreground">
              {isEdit ? 'Attributes for the selected product type.' : 'Select a product type to see its attributes.'}
            </p>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
