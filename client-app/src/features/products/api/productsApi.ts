import { httpClient } from '@/shared/api/httpClient';
import { adaptPagination, type PagedResult } from '@/shared/api/types';
import { etagKey, setEtag, getEtag } from '@/shared/api/etagStore';
import type {
  CreateProductPayload,
  ProductDetail,
  ProductListFilters,
  ProductListItem,
  UpdateProductPayload,
} from '@/features/products/types';

const BASE = '/products';
const RESOURCE = 'product';

// ─── Pagination params ───────────────────────────────────────────────────────
// BE PaginationRequest uses 0-based PageIndex.  FE uses 1-based page numbers.

const buildListParams = (filters: ProductListFilters): Record<string, string | number> => {
  const params: Record<string, string | number> = {
    pageIndex: filters.page - 1,    // convert FE 1-based → BE 0-based
    pageSize: filters.pageSize,
  };
  if (filters.search)       params.search       = filters.search;
  if (filters.categoryId)   params.categoryId   = filters.categoryId;
  if (filters.brandId)      params.brandId      = filters.brandId;
  if (filters.productTypeId) params.productTypeId = filters.productTypeId;
  if (filters.status)       params.status       = filters.status;
  if (typeof filters.minPrice === 'number') params.minPrice = filters.minPrice;
  if (typeof filters.maxPrice === 'number') params.maxPrice = filters.maxPrice;
  // sort matches BE ProductSortOrder enum string values exactly
  if (filters.sort)         params.sort         = filters.sort;
  return params;
};

// ─── Response shapes from BE ─────────────────────────────────────────────────

type BeProductSummary = {
  id: string;
  sku: string;
  name: string;
  slug: string;
  price: number;
  status: string;
  categoryId: string;
  categoryName: string;
  brandId: string;
  brandName: string;
  productTypeId: string;
  productTypeName: string;
  primaryImageUrl?: string | null;
};

type BeProductDto = {
  id: string;
  sku: string;
  name: string;
  slug: string;
  description?: string | null;
  price: number;
  status: string;
  categoryId: string;
  categoryName: string;
  brandId: string;
  brandName: string;
  productTypeId: string;
  productTypeName: string;
  rowVersion: number;
  createdAt?: string | null;
  lastModifiedAt?: string | null;
  attributes: Array<{
    attributeDefinitionId: string;
    key: string;
    label: string;
    value: string;
    dataType: string;
  }>;
  images: Array<{
    id: string;
    url: string;
    alt?: string | null;
    displayOrder: number;
    isPrimary: boolean;
  }>;
};

// ─── Transformers ────────────────────────────────────────────────────────────

const toProductListItem = (dto: BeProductSummary): ProductListItem => ({
  id: dto.id,
  sku: dto.sku,
  name: dto.name,
  slug: dto.slug,
  price: dto.price,
  status: dto.status as ProductDetail['status'],
  categoryName: dto.categoryName,
  brandName: dto.brandName,
  primaryImageUrl: dto.primaryImageUrl,
});

const toProductDetail = (dto: BeProductDto): ProductDetail => ({
  id: dto.id,
  sku: dto.sku,
  name: dto.name,
  slug: dto.slug,
  description: dto.description,
  price: dto.price,
  status: dto.status as ProductDetail['status'],
  category:    { id: dto.categoryId,    name: dto.categoryName    },
  brand:       { id: dto.brandId,       name: dto.brandName       },
  productType: { id: dto.productTypeId, name: dto.productTypeName },
  attributes: dto.attributes.map((a) => ({
    attributeDefinitionId: a.attributeDefinitionId,
    key: a.key,
    label: a.label,
    value: a.value,
    dataType: a.dataType as ProductDetail['attributes'][number]['dataType'],
  })),
  images: dto.images.map((i) => ({
    id: i.id,
    url: i.url,
    alt: i.alt,
    displayOrder: i.displayOrder,
    isPrimary: i.isPrimary,
  })),
  createdAt: dto.createdAt ?? '',
  lastModifiedAt: dto.lastModifiedAt,
  rowVersion: dto.rowVersion,
});

// ─── API functions ────────────────────────────────────────────────────────────

export const fetchProducts = async (
  filters: ProductListFilters,
): Promise<PagedResult<ProductListItem>> => {
  const { data } = await httpClient.get<{ products: { pageIndex: number; pageSize: number; count: number; data: BeProductSummary[] } }>(BASE, {
    params: buildListParams(filters),
  });
  const paginated = adaptPagination(data.products);
  return {
    ...paginated,
    data: paginated.data.map(toProductListItem),
  };
};

export const fetchProductById = async (id: string): Promise<ProductDetail> => {
  const response = await httpClient.get<{ product: BeProductDto }>(`${BASE}/${id}`);
  const etag = (response.headers as Record<string, string>)['etag'];
  setEtag(etagKey(RESOURCE, id), etag);
  return toProductDetail(response.data.product);
};

export const createProduct = async (payload: CreateProductPayload): Promise<{ id: string }> => {
  const { data } = await httpClient.post<{ id: string }>(BASE, payload);
  return data;
};

export const updateProduct = async (
  id: string,
  payload: UpdateProductPayload,
): Promise<{ isSuccess: boolean }> => {
  const ifMatch = getEtag(etagKey(RESOURCE, id));
  const { data } = await httpClient.put<{ isSuccess: boolean }>(`${BASE}/${id}`, payload, {
    headers: ifMatch ? { 'If-Match': ifMatch } : undefined,
  });
  return data;
};

export const deleteProduct = async (id: string): Promise<void> => {
  const ifMatch = getEtag(etagKey(RESOURCE, id));
  await httpClient.delete(`${BASE}/${id}`, {
    headers: ifMatch ? { 'If-Match': ifMatch } : undefined,
  });
};

export const uploadProductImage = async (
  productId: string,
  file: File,
  displayOrder: number,
  isPrimary: boolean,
  onProgress?: (percent: number) => void,
): Promise<void> => {
  const ifMatch = getEtag(etagKey(RESOURCE, productId));
  const form = new FormData();
  form.append('file', file);
  form.append('displayOrder', String(displayOrder));
  form.append('isPrimary', String(isPrimary));
  await httpClient.post(`${BASE}/${productId}/images`, form, {
    headers: {
      'Content-Type': 'multipart/form-data',
      ...(ifMatch ? { 'If-Match': ifMatch } : {}),
    },
    onUploadProgress: (event) => {
      if (!onProgress || !event.total) return;
      onProgress(Math.round((event.loaded / event.total) * 100));
    },
  });
};

export const deleteProductImage = async (productId: string, imageId: string): Promise<void> => {
  const ifMatch = getEtag(etagKey(RESOURCE, productId));
  await httpClient.delete(`${BASE}/${productId}/images/${imageId}`, {
    headers: ifMatch ? { 'If-Match': ifMatch } : undefined,
  });
};
