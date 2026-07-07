const store = new Map<string, string>();

export const etagKey = (resource: string, id: string): string => `${resource}:${id}`;

export const setEtag = (key: string, etag: string | undefined): void => {
  if (!etag) return;
  store.set(key, etag);
};

export const getEtag = (key: string): string | undefined => store.get(key);

export const clearEtag = (key: string): void => {
  store.delete(key);
};
