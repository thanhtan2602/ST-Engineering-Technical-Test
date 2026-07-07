import { httpClient } from '@/shared/api/httpClient';
import { adaptPagination } from '@/shared/api/types';
import type {
  AttributeDefinition,
  AttributeDefinitionPayload,
} from '@/features/attribute-definitions/types';

const BASE = '/attribute-definitions';

export const fetchAttributeDefinitions = async (): Promise<AttributeDefinition[]> => {
  const { data } = await httpClient.get<{
    attributeDefinitions: {
      pageIndex: number;
      pageSize: number;
      count: number;
      data: AttributeDefinition[];
    };
  }>(BASE, { params: { pageIndex: 0, pageSize: 200 } });
  return adaptPagination(data.attributeDefinitions).data;
};

export const createAttributeDefinition = async (
  payload: AttributeDefinitionPayload,
): Promise<{ id: string }> => {
  const { data } = await httpClient.post<{ id: string }>(BASE, payload);
  return data;
};

export const updateAttributeDefinition = async (
  id: string,
  payload: AttributeDefinitionPayload,
): Promise<void> => {
  // BE UpdateAttributeDefinition only accepts: label, unit, allowedValues
  await httpClient.put(`${BASE}/${id}`, {
    label: payload.label,
    unit: payload.unit,
    allowedValues: payload.allowedValues,
  });
};

export const deleteAttributeDefinition = async (id: string): Promise<void> => {
  await httpClient.delete(`${BASE}/${id}`);
};
