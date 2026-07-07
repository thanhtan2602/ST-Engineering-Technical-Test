// FE canonical pagination shape (1-based pageIndex, totalItems, totalPages)
export type PagedResult<T> = {
  data: T[];
  pageIndex: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};

// Raw shape returned by BE PaginatedResult<T> (0-based pageIndex, count)
type BePaginatedResult<T> = {
  pageIndex: number;
  pageSize: number;
  count: number;
  data: T[];
};

export const adaptPagination = <T>(raw: BePaginatedResult<T>): PagedResult<T> => ({
  data: raw.data,
  pageIndex: raw.pageIndex + 1,
  pageSize: raw.pageSize,
  totalItems: Number(raw.count),
  totalPages: raw.count === 0 ? 0 : Math.ceil(Number(raw.count) / raw.pageSize),
});
