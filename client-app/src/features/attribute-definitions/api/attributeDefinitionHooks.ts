import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import {
  createAttributeDefinition,
  deleteAttributeDefinition,
  fetchAttributeDefinitions,
  updateAttributeDefinition,
} from '@/features/attribute-definitions/api/attributeDefinitionsApi';
import { toApiError } from '@/shared/api/problemDetails';
import type { AttributeDefinitionPayload } from '@/features/attribute-definitions/types';

export const attributeDefinitionKeys = {
  all: ['attribute-definitions'] as const,
  list: () => [...attributeDefinitionKeys.all, 'list'] as const,
};

export const useAttributeDefinitionsQuery = () =>
  useQuery({
    queryKey: attributeDefinitionKeys.list(),
    queryFn: fetchAttributeDefinitions,
    staleTime: 5 * 60_000,
  });

export const useCreateAttributeDefinitionMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: (payload: AttributeDefinitionPayload) => createAttributeDefinition(payload),
    onSuccess: () => {
      client.invalidateQueries({ queryKey: attributeDefinitionKeys.list() });
      toast.success('Attribute created');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

type UpdateArgs = { id: string; payload: AttributeDefinitionPayload };

export const useUpdateAttributeDefinitionMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: UpdateArgs) => updateAttributeDefinition(id, payload),
    onSuccess: () => {
      client.invalidateQueries({ queryKey: attributeDefinitionKeys.list() });
      toast.success('Attribute updated');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};

export const useDeleteAttributeDefinitionMutation = () => {
  const client = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteAttributeDefinition(id),
    onSuccess: () => {
      client.invalidateQueries({ queryKey: attributeDefinitionKeys.list() });
      toast.success('Attribute deleted');
    },
    onError: (error) => {
      const apiError = toApiError(error);
      toast.error(apiError.title, { description: apiError.detail });
    },
  });
};
