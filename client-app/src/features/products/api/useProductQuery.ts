import { useQuery } from '@tanstack/react-query';
import { fetchProductById } from '@/features/products/api/productsApi';
import { productKeys } from '@/features/products/api/productKeys';

export const useProductQuery = (id: string | undefined) =>
  useQuery({
    queryKey: id ? productKeys.detail(id) : productKeys.details(),
    queryFn: () => fetchProductById(id as string),
    enabled: Boolean(id),
    staleTime: 60_000,
  });
