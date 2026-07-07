import { ChevronLeft, ChevronRight } from 'lucide-react';
import { Button } from '@/shared/components/Button';

type PaginationProps = {
  pageIndex: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  onPageChange: (page: number) => void;
};

export function Pagination({
  pageIndex,
  pageSize,
  totalItems,
  totalPages,
  onPageChange,
}: PaginationProps) {
  const start = totalItems === 0 ? 0 : (pageIndex - 1) * pageSize + 1;
  const end = Math.min(pageIndex * pageSize, totalItems);

  return (
    <div className="flex items-center justify-between gap-4 py-3 text-sm">
      <p className="text-muted-foreground">
        {start}–{end} / {totalItems}
      </p>
      <div className="flex items-center gap-2">
        <Button
          variant="outline"
          size="sm"
          disabled={pageIndex <= 1}
          onClick={() => onPageChange(pageIndex - 1)}
        >
          <ChevronLeft className="h-4 w-4" />
          Previous
        </Button>
        <span className="min-w-[80px] text-center">
          Page {pageIndex} of {totalPages || 1}
        </span>
        <Button
          variant="outline"
          size="sm"
          disabled={pageIndex >= totalPages}
          onClick={() => onPageChange(pageIndex + 1)}
        >
          Next
          <ChevronRight className="h-4 w-4" />
        </Button>
      </div>
    </div>
  );
}
