import { Link, useNavigate, useParams } from 'react-router-dom';
import { Pencil, Trash2 } from 'lucide-react';
import { useState } from 'react';
import { Button } from '@/shared/components/Button';
import { Badge } from '@/shared/components/Badge';
import { Skeleton } from '@/shared/components/Skeleton';
import { ConfirmDialog } from '@/shared/components/ConfirmDialog';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/Card';
import { PageHeader } from '@/shared/layout/PageHeader';
import { ProductStatusBadge } from '@/features/products/components/ProductStatusBadge';
import { useProductQuery } from '@/features/products/api/useProductQuery';
import { useDeleteProductMutation } from '@/features/products/api/useProductMutations';
import { resolveUploadUrl } from '@/shared/api/httpClient';
import { formatMoney, formatDateTime } from '@/shared/utils/format';

export function ProductDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data: product, isLoading } = useProductQuery(id);
  const deleteMutation = useDeleteProductMutation();
  const [deleteOpen, setDeleteOpen] = useState(false);

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-8 w-64" />
        <Skeleton className="h-48 w-full" />
        <Skeleton className="h-32 w-full" />
      </div>
    );
  }

  if (!product) return null;

  const primaryImage = product.images.find((i) => i.isPrimary) ?? product.images[0];

  return (
    <div>
      <PageHeader
        title={product.name}
        description={`SKU: ${product.sku}`}
        actions={
          <>
            <Button variant="outline" asChild>
              <Link to={`/products/${id}/edit`}>
                <Pencil className="mr-2 h-4 w-4" />
                Edit
              </Link>
            </Button>
            <Button
              variant="destructive"
              onClick={() => setDeleteOpen(true)}
            >
              <Trash2 className="mr-2 h-4 w-4" />
              Delete
            </Button>
          </>
        }
      />

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
        {/* Left: info cards */}
        <div className="space-y-6 lg:col-span-2">
          {/* Overview */}
          <Card>
            <CardHeader>
              <CardTitle>Overview</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="grid grid-cols-2 gap-x-6 gap-y-3 text-sm">
                <DetailRow label="Status">
                  <ProductStatusBadge status={product.status} />
                </DetailRow>
                <DetailRow label="Price">{formatMoney(product.price)}</DetailRow>
                <DetailRow label="Category">{product.category.name}</DetailRow>
                <DetailRow label="Brand">{product.brand.name}</DetailRow>
                <DetailRow label="Product Type">{product.productType.name}</DetailRow>
                <DetailRow label="Slug">
                  <code className="text-xs">{product.slug}</code>
                </DetailRow>
                {product.description ? (
                  <div className="col-span-2">
                    <dt className="font-medium text-muted-foreground">Description</dt>
                    <dd className="mt-1 whitespace-pre-wrap">{product.description}</dd>
                  </div>
                ) : null}
              </dl>
            </CardContent>
          </Card>

          {/* Attributes */}
          {product.attributes.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle>Attributes</CardTitle>
              </CardHeader>
              <CardContent>
                <dl className="grid grid-cols-2 gap-x-6 gap-y-3 text-sm">
                  {product.attributes.map((attr) => (
                    <DetailRow key={attr.attributeDefinitionId} label={attr.label}>
                      <span className="flex items-center gap-1.5">
                        {attr.value}
                        <Badge variant="outline" className="text-[10px]">
                          {attr.dataType}
                        </Badge>
                      </span>
                    </DetailRow>
                  ))}
                </dl>
              </CardContent>
            </Card>
          )}

          {/* Audit */}
          <Card>
            <CardHeader>
              <CardTitle>Audit</CardTitle>
            </CardHeader>
            <CardContent>
              <dl className="grid grid-cols-2 gap-x-6 gap-y-3 text-sm">
                <DetailRow label="Created">{formatDateTime(product.createdAt)}</DetailRow>
                <DetailRow label="Last modified">{formatDateTime(product.lastModifiedAt)}</DetailRow>
              </dl>
            </CardContent>
          </Card>
        </div>

        {/* Right: images */}
        <div className="space-y-3">
          {primaryImage ? (
            <img
              src={resolveUploadUrl(primaryImage.url)}
              alt={primaryImage.alt ?? product.name}
              className="w-full rounded-lg object-cover"
            />
          ) : (
            <div className="flex aspect-square w-full items-center justify-center rounded-lg border bg-muted text-sm text-muted-foreground">
              No image
            </div>
          )}
          {product.images.length > 1 && (
            <div className="grid grid-cols-4 gap-2">
              {product.images.slice(1).map((img) => (
                <img
                  key={img.id}
                  src={resolveUploadUrl(img.url)}
                  alt={img.alt ?? ''}
                  className="aspect-square w-full rounded-md object-cover"
                />
              ))}
            </div>
          )}
        </div>
      </div>

      <ConfirmDialog
        open={deleteOpen}
        title="Delete product"
        description={`Are you sure you want to delete "${product.name}"? This action cannot be undone.`}
        confirmLabel="Delete"
        variant="destructive"
        loading={deleteMutation.isPending}
        onOpenChange={setDeleteOpen}
        onConfirm={() => {
          deleteMutation.mutate(id!, {
            onSuccess: () => navigate('/products'),
          });
        }}
      />
    </div>
  );
}

type DetailRowProps = {
  label: string;
  children: React.ReactNode;
};

function DetailRow({ label, children }: DetailRowProps) {
  return (
    <div>
      <dt className="font-medium text-muted-foreground">{label}</dt>
      <dd className="mt-0.5">{children}</dd>
    </div>
  );
}
