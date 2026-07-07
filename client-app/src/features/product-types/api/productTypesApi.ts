import { httpClient } from '@/shared/api/httpClient';
import { adaptPagination } from '@/shared/api/types';
import type {
  ProductType,
  ProductTypeSummary,
  ProductTypePayload,
} from '@/features/product-types/types';

const BASE = '/product-types';

export const fetchProductTypes = async (): Promise<ProductTypeSummary[]> => {
  const { data } = await httpClient.get<{
    productTypes: {
      pageIndex: number;
      pageSize: number;
      count: number;
      data: ProductTypeSummary[];
    };
  }>(BASE, { params: { pageIndex: 0, pageSize: 200 } });
  return adaptPagination(data.productTypes).data;
};

export const fetchProductTypeById = async (id: string): Promise<ProductType> => {
  const { data } = await httpClient.get<{ productType: ProductType }>(`${BASE}/${id}`);
  return data.productType;
};

export const createProductType = async (payload: ProductTypePayload): Promise<{ id: string }> => {
  const { data } = await httpClient.post<{ id: string }>(BASE, payload);
  return data;
};

export const updateProductType = async (
  id: string,
  payload: ProductTypePayload,
): Promise<void> => {
  await httpClient.put(`${BASE}/${id}`, payload);
};

export const deleteProductType = async (id: string): Promise<void> => {
  await httpClient.delete(`${BASE}/${id}`);
};

export const attachAttributeToProductType = async (
  productTypeId: string,
  attributeDefinitionId: string,
  isRequired: boolean,
  displayOrder: number,
): Promise<void> => {
  await httpClient.post(`${BASE}/${productTypeId}/attributes`, {
    attributeDefinitionId,
    isRequired,
    displayOrder,
  });
};

export const detachAttributeFromProductType = async (
  productTypeId: string,
  attributeDefinitionId: string,
): Promise<void> => {
  await httpClient.delete(`${BASE}/${productTypeId}/attributes/${attributeDefinitionId}`);
};
