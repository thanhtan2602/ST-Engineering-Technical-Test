// Matches BE BrandDto: id, name, slug
export type Brand = {
  id: string;
  name: string;
  slug: string;
};

export type BrandPayload = {
  name: string;
  slug: string;
};
