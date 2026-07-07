import { Moon, Sun } from 'lucide-react';
import { Button } from '@/shared/components/Button';
import { useUiStore } from '@/shared/stores/uiStore';

export function Topbar() {
  const theme = useUiStore((state) => state.theme);
  const toggleTheme = useUiStore((state) => state.toggleTheme);
  const appName = import.meta.env.VITE_APP_NAME;

  return (
    <header className="flex h-16 items-center justify-between border-b bg-card px-6">
      <h1 className="text-sm font-semibold text-muted-foreground">{appName}</h1>
      <div className="flex items-center gap-2">
        <Button variant="ghost" size="icon" onClick={toggleTheme} aria-label="Toggle theme">
          {theme === 'dark' ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
        </Button>
      </div>
    </header>
  );
}
