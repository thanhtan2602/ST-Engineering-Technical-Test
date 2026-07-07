import { useEffect } from 'react';
import { useUiStore } from '@/shared/stores/uiStore';

export function ThemeSync() {
  const theme = useUiStore((state) => state.theme);

  useEffect(() => {
    const root = document.documentElement;
    root.classList.toggle('dark', theme === 'dark');
  }, [theme]);

  return null;
}
