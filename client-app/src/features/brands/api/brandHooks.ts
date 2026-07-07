import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import {
  createBrand,
  deleteBrand,
  fetchBrands,
  updateBrand,
} from '@/features/brands/api/brandsApi';
import { toApiError } from '@/shared/api/problemDetails';
import type { BrandPayload } from '@/features/brands/types';

export const brandKeys = {
  all: ['brands'] as const,
  list: () => [...brandKeys.all, 'list'] as const,
};

export const useBrandsQuery = () =>
  useQuery({
    queryKey: brandKeys.list(),
    queryFn: fetchBrands,
    staleTime: 5 * 60_000,
  });

export const useCreateBrandMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: (payload: BrandPayload) => createBrand(payload),
    onSuccess: () => {
      client.invalidateQueries({ queryKey: brandKeys.list() });
      toast.success('Brand created');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

type UpdateArgs = { id: string; payload: BrandPayload };

export const useUpdateBrandMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: UpdateArgs) => updateBrand(id, payload),
    onSuccess: () => {
      client.invalidateQueries({ queryKey: brandKeys.list() });
      toast.success('Brand updated');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

export const useDeleteBrandMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteBrand(id),
    onSuccess: () => {
      client.invalidateQueries({ queryKey: brandKeys.list() });
      toast.success('Brand deleted');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};
