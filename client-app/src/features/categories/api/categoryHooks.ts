import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import {
  createCategory,
  deleteCategory,
  fetchCategories,
  updateCategory,
} from '@/features/categories/api/categoriesApi';
import { toApiError } from '@/shared/api/problemDetails';
import type { CategoryPayload } from '@/features/categories/types';

export const categoryKeys = {
  all: ['categories'] as const,
  list: () => [...categoryKeys.all, 'list'] as const,
};

export const useCategoriesQuery = () =>
  useQuery({
    queryKey: categoryKeys.list(),
    queryFn: fetchCategories,
    staleTime: 5 * 60_000,
  });

export const useCreateCategoryMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: (payload: CategoryPayload) => createCategory(payload),
    onSuccess: () => {
      client.invalidateQueries({ queryKey: categoryKeys.list() });
      toast.success('Category created');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

type UpdateArgs = { id: string; payload: CategoryPayload };

export const useUpdateCategoryMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: UpdateArgs) => updateCategory(id, payload),
    onSuccess: () => {
      client.invalidateQueries({ queryKey: categoryKeys.list() });
      toast.success('Category updated');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

export const useDeleteCategoryMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteCategory(id),
    onSuccess: () => {
      client.invalidateQueries({ queryKey: categoryKeys.list() });
      toast.success('Category deleted');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};
