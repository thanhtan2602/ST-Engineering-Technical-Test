import axios, { type AxiosInstance } from 'axios';

const createHttpClient = (): AxiosInstance => {
  const instance = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    timeout: 20_000,
    headers: {
      Accept: 'application/json',
    },
  });

  instance.interceptors.request.use((config) => {
    config.headers.set('X-Client', 'fashion-shop-web');
    return config;
  });

  return instance;
};

export const httpClient = createHttpClient();

export const uploadsBaseUrl = import.meta.env.VITE_UPLOADS_BASE_URL;

export const resolveUploadUrl = (path: string): string => {
  if (!path) return '';
  if (path.startsWith('http://') || path.startsWith('https://')) return path;
  const base = uploadsBaseUrl.replace(/\/$/, '');
  const rel = path.startsWith('/') ? path : `/${path}`;
  return `${base}${rel}`;
};
