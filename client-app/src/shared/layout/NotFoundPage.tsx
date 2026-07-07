import { Link } from 'react-router-dom';
import { Button } from '@/shared/components/Button';

export function NotFoundPage() {
  return (
    <div className="flex min-h-[60vh] flex-col items-center justify-center gap-4 text-center">
      <h1 className="text-4xl font-semibold">404</h1>
      <p className="text-sm text-muted-foreground">
        The page you were looking for does not exist.
      </p>
      <Button asChild>
        <Link to="/products">Back to products</Link>
      </Button>
    </div>
  );
}
