import type { AxiosError } from 'axios';

export type ProblemDetails = {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
};

export type ApiError = {
  status: number;
  title: string;
  detail?: string;
  errors: Record<string, string[]>;
  traceId?: string;
  isConcurrencyConflict: boolean;
  isValidation: boolean;
  isNotFound: boolean;
};

const isProblemDetails = (data: unknown): data is ProblemDetails =>
  typeof data === 'object' && data !== null && ('title' in data || 'status' in data);

export const toApiError = (error: unknown): ApiError => {
  const axiosError = error as AxiosError<ProblemDetails>;
  const response = axiosError?.response;

  if (response && isProblemDetails(response.data)) {
    const status = response.data.status ?? response.status ?? 500;
    return {
      status,
      title: response.data.title ?? defaultTitleForStatus(status),
      detail: response.data.detail,
      errors: response.data.errors ?? {},
      traceId: response.data.traceId,
      isConcurrencyConflict: status === 409,
      isValidation: status === 400,
      isNotFound: status === 404,
    };
  }

  if (response) {
    return {
      status: response.status,
      title: defaultTitleForStatus(response.status),
      errors: {},
      isConcurrencyConflict: response.status === 409,
      isValidation: response.status === 400,
      isNotFound: response.status === 404,
    };
  }

  return {
    status: 0,
    title: 'Network error',
    detail: axiosError?.message ?? 'Unable to reach the server.',
    errors: {},
    isConcurrencyConflict: false,
    isValidation: false,
    isNotFound: false,
  };
};

const defaultTitleForStatus = (status: number): string => {
  if (status >= 500) return 'Server error';
  if (status === 404) return 'Not found';
  if (status === 409) return 'Conflict';
  if (status === 400) return 'Invalid input';
  if (status === 401) return 'Unauthorized';
  if (status === 403) return 'Forbidden';
  return 'Request failed';
};
