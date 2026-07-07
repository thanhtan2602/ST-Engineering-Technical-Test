// Matches BE CategoryDto: id, name, slug, parentId
export type Category = {
  id: string;
  name: string;
  slug: string;
  parentId?: string | null;
};

export type CategoryPayload = {
  name: string;
  slug: string;
  parentId?: string | null;
};
