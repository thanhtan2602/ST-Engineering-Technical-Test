// ProductStatus matches BE enum (JsonStringEnumConverter): Draft | Active | Inactive
export type ProductStatus = 'Draft' | 'Active' | 'Inactive';

// AttributeDataType matches BE enum: Text | Number | Boolean | Enum | Date
export type AttributeDataType = 'Text' | 'Number' | 'Boolean' | 'Enum' | 'Date';

export type LookupItem = {
  id: string;
  name: string;
};

export type ProductAttributeView = {
  attributeDefinitionId: string;
  key: string;
  label: string;
  dataType: AttributeDataType;
  value: string;
};

export type ProductImageView = {
  id: string;
  url: string;
  alt?: string | null;
  displayOrder: number;
  isPrimary: boolean;
};

export type ProductListItem = {
  id: string;
  sku: string;
  name: string;
  slug: string;
  price: number;
  status: ProductStatus;
  categoryName: string;
  brandName: string;
  primaryImageUrl?: string | null;
};

export type ProductDetail = {
  id: string;
  sku: string;
  name: string;
  slug: string;
  description?: string | null;
  price: number;
  status: ProductStatus;
  category: LookupItem;
  brand: LookupItem;
  productType: LookupItem;
  attributes: ProductAttributeView[];
  images: ProductImageView[];
  createdAt: string;
  lastModifiedAt?: string | null;
  rowVersion: number;
};

// Sort values match BE ProductSortOrder enum (JsonStringEnumConverter)
export type ProductSortOrder = 'Newest' | 'PriceAsc' | 'PriceDesc' | 'NameAsc' | 'NameDesc';

export type ProductListFilters = {
  search?: string;
  categoryId?: string;
  brandId?: string;
  productTypeId?: string;
  status?: ProductStatus;
  minPrice?: number;
  maxPrice?: number;
  sort?: ProductSortOrder;
  page: number;
  pageSize: number;
};

export type CreateProductPayload = {
  sku: string;
  name: string;
  slug: string;
  description?: string;
  price: number;
  status: ProductStatus;
  categoryId: string;
  brandId: string;
  productTypeId: string;
  attributes: Array<{ attributeDefinitionId: string; value: string }>;
};

export type UpdateProductPayload = Omit<CreateProductPayload, 'productTypeId' | 'sku'>;
