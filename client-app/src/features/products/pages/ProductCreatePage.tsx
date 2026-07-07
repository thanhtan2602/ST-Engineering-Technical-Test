import { useNavigate } from 'react-router-dom';
import { useForm, FormProvider } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { toast } from 'sonner';
import { Button } from '@/shared/components/Button';
import { PageHeader } from '@/shared/layout/PageHeader';
import { ProductForm } from '@/features/products/components/ProductForm';
import { productFormSchema, type ProductFormValues } from '@/features/products/schemas';
import { useCreateProductMutation } from '@/features/products/api/useProductMutations';
import { toApiError } from '@/shared/api/problemDetails';

const DEFAULT_VALUES: ProductFormValues = {
  sku: '',
  name: '',
  slug: '',
  description: '',
  price: 0,
  status: 'Draft',
  categoryId: '',
  brandId: '',
  productTypeId: '',
  attributes: [],
};

export function ProductCreatePage() {
  const navigate = useNavigate();
  const createMutation = useCreateProductMutation();

  const methods = useForm<ProductFormValues>({
    resolver: zodResolver(productFormSchema),
    defaultValues: DEFAULT_VALUES,
    mode: 'onBlur',
  });

  const {
    handleSubmit,
    setError,
    formState: { isSubmitting },
  } = methods;

  const onSubmit = handleSubmit(async (values) => {
    createMutation.mutate(
      {
        sku: values.sku.toUpperCase(),
        name: values.name,
        slug: values.slug,
        description: values.description || undefined,
        price: values.price,
        status: values.status,
        categoryId: values.categoryId,
        brandId: values.brandId,
        productTypeId: values.productTypeId,
        attributes: values.attributes.filter((a) => a.value.trim() !== ''),
      },
      {
        onSuccess: (product) => {
          navigate(`/products/${product.id}`);
        },
        onError: (error) => {
          const apiError = toApiError(error);
          if (apiError.isValidation) {
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
      <PageHeader title="New Product" />
      <FormProvider {...methods}>
        <form onSubmit={onSubmit}>
          <ProductForm />
          <div className="mt-6 flex items-center justify-end gap-3 border-t pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => navigate('/products')}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={isSubmitting || createMutation.isPending}>
              {createMutation.isPending ? 'Creating…' : 'Create product'}
            </Button>
          </div>
        </form>
      </FormProvider>
    </div>
  );
}
