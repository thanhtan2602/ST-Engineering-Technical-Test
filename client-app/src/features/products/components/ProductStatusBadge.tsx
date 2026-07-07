import { Badge } from '@/shared/components/Badge';
import type { ProductStatus } from '@/features/products/types';

type ProductStatusBadgeProps = {
  status: ProductStatus;
};

const statusConfig: Record<ProductStatus, { label: string; variant: 'success' | 'secondary' | 'outline' }> = {
  Active:   { label: 'Active',   variant: 'success'   },
  Draft:    { label: 'Draft',    variant: 'secondary' },
  Inactive: { label: 'Inactive', variant: 'outline'   },
};

export function ProductStatusBadge({ status }: ProductStatusBadgeProps) {
  const config = statusConfig[status] ?? { label: status, variant: 'secondary' };
  return <Badge variant={config.variant}>{config.label}</Badge>;
}
