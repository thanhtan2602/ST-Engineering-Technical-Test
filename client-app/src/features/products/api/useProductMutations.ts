import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import {
  createProduct,
  deleteProduct,
  deleteProductImage,
  updateProduct,
  uploadProductImage,
} from '@/features/products/api/productsApi';
import { productKeys } from '@/features/products/api/productKeys';
import { toApiError } from '@/shared/api/problemDetails';
import type { CreateProductPayload, UpdateProductPayload } from '@/features/products/types';

export const useCreateProductMutation = () => {
  const client = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateProductPayload) => createProduct(payload),
    onSuccess: () => {
      client.invalidateQueries({ queryKey: productKeys.lists() });
      toast.success('Product created');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

type UpdateArgs = {
  id: string;
  payload: UpdateProductPayload;
};

export const useUpdateProductMutation = () => {
  const client = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payload }: UpdateArgs) => updateProduct(id, payload),
    onSuccess: (_, { id }) => {
      client.invalidateQueries({ queryKey: productKeys.lists() });
      client.invalidateQueries({ queryKey: productKeys.detail(id) });
      toast.success('Product updated');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      if (apiError.isConcurrencyConflict) {
        toast.error('Conflict', {
          description:
            'This product was changed by someone else. Reload to see the latest version.',
        });
      } else {
        toast.error(apiError.title, { description: apiError.detail });
      }
    },
  });
};

export const useDeleteProductMutation = () => {
  const client = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => deleteProduct(id),
    onSuccess: (_, id) => {
      client.invalidateQueries({ queryKey: productKeys.lists() });
      client.removeQueries({ queryKey: productKeys.detail(id) });
      toast.success('Product deleted');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

type UploadArgs = {
  productId: string;
  file: File;
  displayOrder: number;
  isPrimary: boolean;
  onProgress?: (percent: number) => void;
};

export const useUploadProductImageMutation = () => {
  const client = useQueryClient();

  return useMutation({
    mutationFn: ({ productId, file, displayOrder, isPrimary, onProgress }: UploadArgs) =>
      uploadProductImage(productId, file, displayOrder, isPrimary, onProgress),
    onSuccess: (_, { productId }) => {
      client.invalidateQueries({ queryKey: productKeys.detail(productId) });
      client.invalidateQueries({ queryKey: productKeys.lists() });
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

type DeleteImageArgs = {
  productId: string;
  imageId: string;
};

export const useDeleteProductImageMutation = () => {
  const client = useQueryClient();

  return useMutation({
    mutationFn: ({ productId, imageId }: DeleteImageArgs) =>
      deleteProductImage(productId, imageId),
    onSuccess: (_, { productId }) => {
      client.invalidateQueries({ queryKey: productKeys.detail(productId) });
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};
