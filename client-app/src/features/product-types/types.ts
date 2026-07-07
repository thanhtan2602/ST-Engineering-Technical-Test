import type { AttributeDataType } from '@/features/products/types';

// Matches BE ProductTypeAttributeDto: key, label, dataType, unit, isRequired, displayOrder, allowedValues
export type ProductTypeAttribute = {
  attributeDefinitionId: string;
  key: string;
  label: string;
  dataType: AttributeDataType;
  unit?: string | null;
  isRequired: boolean;
  displayOrder: number;
  allowedValues: string[];
};

// Matches BE ProductTypeDto: id, code, name (no slug)
export type ProductType = {
  id: string;
  code: string;
  name: string;
  attributes: ProductTypeAttribute[];
};

export type ProductTypeSummary = {
  id: string;
  code: string;
  name: string;
};

// Matches BE CreateProductTypeRequest: code, name (no slug)
export type ProductTypePayload = {
  code: string;
  name: string;
};
