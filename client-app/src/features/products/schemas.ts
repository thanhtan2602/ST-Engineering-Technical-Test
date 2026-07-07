import { z } from 'zod';

// Matches BE ProductStatus enum values (JsonStringEnumConverter)
export const productStatusSchema = z.enum(['Draft', 'Active', 'Inactive']);

export const productAttributeInputSchema = z.object({
  attributeDefinitionId: z.string().uuid(),
  value: z.string().trim().min(1, 'Value is required'),
});

export const productFormSchema = z.object({
  sku: z
    .string()
    .trim()
    .min(1, 'SKU is required')
    .max(64, 'SKU must be at most 64 characters')
    .regex(/^[A-Z0-9-]+$/i, 'SKU may contain letters, digits and dashes only'),
  name: z.string().trim().min(1, 'Name is required').max(200, 'Name too long'),
  slug: z
    .string()
    .trim()
    .min(1, 'Slug is required')
    .max(220, 'Slug too long')
    .regex(/^[a-z0-9-]+$/, 'Slug must be lowercase kebab-case'),
  description: z.string().max(4000, 'Too long').optional().or(z.literal('')),
  price: z.coerce.number().min(0, 'Price must be ≥ 0'),
  status: productStatusSchema,
  categoryId: z.string().uuid('Choose a category'),
  brandId: z.string().uuid('Choose a brand'),
  productTypeId: z.string().uuid('Choose a product type'),
  attributes: z.array(productAttributeInputSchema),
});

export type ProductFormValues = z.infer<typeof productFormSchema>;
