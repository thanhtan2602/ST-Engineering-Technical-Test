import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useForm, FormProvider } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { toast } from 'sonner';
import { Button } from '@/shared/components/Button';
import { Skeleton } from '@/shared/components/Skeleton';
import { PageHeader } from '@/shared/layout/PageHeader';
import { ProductForm } from '@/features/products/components/ProductForm';
import { ImageUploader } from '@/features/products/components/ImageUploader';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/Card';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '@/shared/components/Dialog';
import { productFormSchema, type ProductFormValues } from '@/features/products/schemas';
import { useProductQuery } from '@/features/products/api/useProductQuery';
import { useUpdateProductMutation } from '@/features/products/api/useProductMutations';
import { useCategoriesQuery } from '@/features/categories/api/categoryHooks';
import { useBrandsQuery } from '@/features/brands/api/brandHooks';
import { useProductTypeQuery } from '@/features/product-types/api/productTypeHooks';
import { toApiError } from '@/shared/api/problemDetails';
import type { ProductDetail } from '@/features/products/types';

const toFormValues = (product: ProductDetail): ProductFormValues => ({
  sku: product.sku,
  name: product.name,
  slug: product.slug,
  description: product.description ?? '',
  price: product.price,
  status: product.status,
  categoryId: product.category.id,
  brandId: product.brand.id,
  productTypeId: product.productType.id,
  attributes: product.attributes.map((a) => ({
    attributeDefinitionId: a.attributeDefinitionId,
    value: a.value,
  })),
});

export function ProductEditPage() {
  const { id } = useParams<{ id: string }>();
  const { data: product, isLoading: productLoading } = useProductQuery(id);
  const { data: categories, isLoading: catLoading } = useCategoriesQuery();
  const { data: brands, isLoading: brandLoading } = useBrandsQuery();
  const { data: productType, isLoading: ptLoading } = useProductTypeQuery(product?.productType.id);

  const isLoading =
    productLoading || catLoading || brandLoading || (product ? ptLoading : false);

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-64 w-full" />
      </div>
    );
  }

  if (!product || !categories || !brands || !productType) return null;

  return <ProductEditForm product={product} />;
}

type ProductEditFormProps = {
  product: ProductDetail;
};

function ProductEditForm({ product }: ProductEditFormProps) {
  const navigate = useNavigate();
  const updateMutation = useUpdateProductMutation();
  const [conflictDialogOpen, setConflictDialogOpen] = useState(false);

  const methods = useForm<ProductFormValues>({
    resolver: zodResolver(productFormSchema),
    mode: 'onBlur',
    defaultValues: toFormValues(product),
  });

  const {
    handleSubmit,
    setError,
    formState: { isSubmitting },
  } = methods;

  const onSubmit = handleSubmit(async (values) => {
    updateMutation.mutate(
      {
        id: product.id,
        payload: {
          name: values.name,
          slug: values.slug,
          description: values.description || undefined,
          price: values.price,
          status: values.status,
          categoryId: values.categoryId,
          brandId: values.brandId,
          attributes: values.attributes.filter((a) => a.value.trim() !== ''),
        },
      },
      {
        onSuccess: () => {
          navigate(`/products/${product.id}`);
        },
        onError: (error) => {
          const apiError = toApiError(error);
          if (apiError.isConcurrencyConflict) {
            setConflictDialogOpen(true);
          } else if (apiError.isValidation) {
            Object.entries(apiError.errors).forEach(([field, messages]) => {
              const key = (field.charAt(0).toLowerCase() + field.slice(1)) as keyof ProductFormValues;
              setError(key, { message: messages[0] });
            });
          } else {
            toast.error(apiError.title, { description: apiError.detail });
          }
        },
      },
    );
  });

  return (
    <div>
      <PageHeader title={`Edit: ${product.name}`} />

      <FormProvider {...methods}>
        <form onSubmit={onSubmit}>
          <ProductForm isEdit />

          <Card className="mt-6">
            <CardHeader>
              <CardTitle>Images</CardTitle>
            </CardHeader>
            <CardContent>
              <ImageUploader productId={product.id} images={product.images} />
            </CardContent>
          </Card>

          <div className="mt-6 flex items-center justify-end gap-3 border-t pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => navigate(`/products/${product.id}`)}
            >
              Back
            </Button>
            <Button
              type="submit"
              disabled={isSubmitting || updateMutation.isPending}
            >
              {updateMutation.isPending ? 'Saving…' : 'Save changes'}
            </Button>
          </div>
        </form>
      </FormProvider>

      <Dialog open={conflictDialogOpen} onOpenChange={setConflictDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Conflict detected</DialogTitle>
            <DialogDescription>
              This product was updated by someone else since you opened the page. Reload to see
              the latest version before making changes.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setConflictDialogOpen(false)}>
              Stay here
            </Button>
            <Button
              onClick={() => {
                setConflictDialogOpen(false);
                navigate(0);
              }}
            >
              Reload
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
