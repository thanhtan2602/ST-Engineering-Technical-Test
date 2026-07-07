import type { AttributeDataType } from '@/features/products/types';

// Field names match BE AttributeDefinitionDto: key, label (not code, name)
export type AttributeDefinition = {
  id: string;
  key: string;
  label: string;
  dataType: AttributeDataType;
  unit?: string | null;
  allowedValues?: string[] | null;
};

export type AttributeDefinitionPayload = {
  key: string;
  label: string;
  dataType: AttributeDataType;
  unit?: string;
  allowedValues?: string[];
};
