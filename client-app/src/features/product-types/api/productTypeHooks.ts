import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import {
  attachAttributeToProductType,
  createProductType,
  deleteProductType,
  detachAttributeFromProductType,
  fetchProductTypeById,
  fetchProductTypes,
  updateProductType,
} from '@/features/product-types/api/productTypesApi';
import { toApiError } from '@/shared/api/problemDetails';
import type { ProductTypePayload } from '@/features/product-types/types';

export const productTypeKeys = {
  all: ['product-types'] as const,
  list: () => [...productTypeKeys.all, 'list'] as const,
  detail: (id: string) => [...productTypeKeys.all, 'detail', id] as const,
};

export const useProductTypesQuery = () =>
  useQuery({
    queryKey: productTypeKeys.list(),
    queryFn: fetchProductTypes,
    staleTime: 5 * 60_000,
  });

export const useProductTypeQuery = (id: string | undefined) =>
  useQuery({
    queryKey: id ? productTypeKeys.detail(id) : productTypeKeys.all,
    queryFn: () => fetchProductTypeById(id as string),
    enabled: Boolean(id),
    staleTime: 5 * 60_000,
  });

export const useCreateProductTypeMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: (payload: ProductTypePayload) => createProductType(payload),
    onSuccess: () => {
      client.invalidateQueries({ queryKey: productTypeKeys.list() });
      toast.success('Product type created');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

type UpdateArgs = { id: string; payload: ProductTypePayload };

export const useUpdateProductTypeMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: UpdateArgs) => updateProductType(id, payload),
    onSuccess: (_, { id }) => {
      client.invalidateQueries({ queryKey: productTypeKeys.list() });
      client.invalidateQueries({ queryKey: productTypeKeys.detail(id) });
      toast.success('Product type updated');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

export const useDeleteProductTypeMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteProductType(id),
    onSuccess: () => {
      client.invalidateQueries({ queryKey: productTypeKeys.list() });
      toast.success('Product type deleted');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

type AttachArgs = {
  productTypeId: string;
  attributeDefinitionId: string;
  isRequired: boolean;
  displayOrder: number;
};

export const useAttachAttributeMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: ({ productTypeId, attributeDefinitionId, isRequired, displayOrder }: AttachArgs) =>
      attachAttributeToProductType(productTypeId, attributeDefinitionId, isRequired, displayOrder),
    onSuccess: (_, { productTypeId }) => {
      client.invalidateQueries({ queryKey: productTypeKeys.detail(productTypeId) });
      client.invalidateQueries({ queryKey: productTypeKeys.list() });
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

type DetachArgs = { productTypeId: string; attributeDefinitionId: string };

export const useDetachAttributeMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: ({ productTypeId, attributeDefinitionId }: DetachArgs) =>
      detachAttributeFromProductType(productTypeId, attributeDefinitionId),
    onSuccess: (_, { productTypeId }) => {
      client.invalidateQueries({ queryKey: productTypeKeys.detail(productTypeId) });
      client.invalidateQueries({ queryKey: productTypeKeys.list() });
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};
