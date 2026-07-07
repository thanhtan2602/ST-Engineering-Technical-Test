import { httpClient } from '@/shared/api/httpClient';
import { adaptPagination } from '@/shared/api/types';
import type { Brand, BrandPayload } from '@/features/brands/types';

const BASE = '/brands';

export const fetchBrands = async (): Promise<Brand[]> => {
  const { data } = await httpClient.get<{
    brands: { pageIndex: number; pageSize: number; count: number; data: Brand[] };
  }>(BASE, { params: { pageIndex: 0, pageSize: 200 } });
  return adaptPagination(data.brands).data;
};

export const createBrand = async (payload: BrandPayload): Promise<{ id: string }> => {
  const { data } = await httpClient.post<{ id: string }>(BASE, payload);
  return data;
};

export const updateBrand = async (id: string, payload: BrandPayload): Promise<void> => {
  await httpClient.put(`${BASE}/${id}`, payload);
};

export const deleteBrand = async (id: string): Promise<void> => {
  await httpClient.delete(`${BASE}/${id}`);
};
