import { useRouteError, isRouteErrorResponse, Link } from 'react-router-dom';
import { Button } from '@/shared/components/Button';

export function ErrorBoundary() {
  const error = useRouteError();
  const message = isRouteErrorResponse(error)
    ? `${error.status} ${error.statusText}`
    : error instanceof Error
      ? error.message
      : 'Unknown error';

  return (
    <div className="flex min-h-[60vh] flex-col items-center justify-center gap-4 text-center">
      <h1 className="text-3xl font-semibold">Something went wrong</h1>
      <p className="max-w-md text-sm text-muted-foreground">{message}</p>
      <Button asChild>
        <Link to="/products">Back to products</Link>
      </Button>
    </div>
  );
}
