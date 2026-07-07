import { httpClient } from '@/shared/api/httpClient';
import { adaptPagination } from '@/shared/api/types';
import type { Category, CategoryPayload } from '@/features/categories/types';

const BASE = '/categories';

export const fetchCategories = async (): Promise<Category[]> => {
  // Fetch a large page to get all categories (no infinite scroll needed for selects)
  const { data } = await httpClient.get<{
    categories: { pageIndex: number; pageSize: number; count: number; data: Category[] };
  }>(BASE, { params: { pageIndex: 0, pageSize: 200 } });
  return adaptPagination(data.categories).data;
};

export const createCategory = async (payload: CategoryPayload): Promise<{ id: string }> => {
  const { data } = await httpClient.post<{ id: string }>(BASE, payload);
  return data;
};

export const updateCategory = async (id: string, payload: CategoryPayload): Promise<void> => {
  await httpClient.put(`${BASE}/${id}`, payload);
};

export const deleteCategory = async (id: string): Promise<void> => {
  await httpClient.delete(`${BASE}/${id}`);
};
