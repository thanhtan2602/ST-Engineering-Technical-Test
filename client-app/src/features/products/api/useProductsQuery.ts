import { useQuery, keepPreviousData } from '@tanstack/react-query';
import { fetchProducts } from '@/features/products/api/productsApi';
import { productKeys } from '@/features/products/api/productKeys';
import type { ProductListFilters } from '@/features/products/types';

export const useProductsQuery = (filters: ProductListFilters) =>
  useQuery({
    queryKey: productKeys.list(filters),
    queryFn: () => fetchProducts(filters),
    placeholderData: keepPreviousData,
    staleTime: 30_000,
  });
