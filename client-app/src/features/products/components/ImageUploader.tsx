import { useCallback, useState } from 'react';
import { useDropzone, type FileRejection } from 'react-dropzone';
import { Upload, X, Loader2, Info } from 'lucide-react';
import { Button } from '@/shared/components/Button';
import { ConfirmDialog } from '@/shared/components/ConfirmDialog';
import {
  useDeleteProductImageMutation,
  useUploadProductImageMutation,
} from '@/features/products/api/useProductMutations';
import { resolveUploadUrl } from '@/shared/api/httpClient';
import { cn } from '@/shared/utils/cn';
import type { ProductImageView } from '@/features/products/types';

const ACCEPTED_TYPES: Record<string, string[]> = {
  'image/jpeg': ['.jpg', '.jpeg'],
  'image/png': ['.png'],
  'image/webp': ['.webp'],
};
const MAX_SIZE = 5 * 1024 * 1024;

type UploadingFile = {
  name: string;
  progress: number;
  error?: string;
};

type ImageUploaderProps = {
  productId: string;
  images: ProductImageView[];
};

export function ImageUploader({ productId, images }: ImageUploaderProps) {
  const [uploading, setUploading] = useState<UploadingFile[]>([]);
  const [pendingDeleteId, setPendingDeleteId] = useState<string | null>(null);
  const uploadMutation = useUploadProductImageMutation();
  const deleteMutation = useDeleteProductImageMutation();

  const onDrop = useCallback(
    (accepted: File[], rejected: FileRejection[]) => {
      if (images.length + accepted.length > 5) {
        alert('You can upload at most 5 images per product.');
        return;
      }

      rejected.forEach((r) => {
        const msg = r.errors.map((e) => e.message).join(', ');
        setUploading((prev) => [...prev, { name: r.file.name, progress: 0, error: msg }]);
        setTimeout(
          () =>
            setUploading((prev) => prev.filter((u) => u.name !== r.file.name || u.error !== msg)),
          4000,
        );
      });

      accepted.forEach((file) => {
        setUploading((prev) => [...prev, { name: file.name, progress: 0 }]);
        uploadMutation.mutate(
          {
            productId,
            file,
            displayOrder: images.length,
            isPrimary: images.length === 0,
            onProgress: (percent) =>
              setUploading((prev) =>
                prev.map((u) => (u.name === file.name ? { ...u, progress: percent } : u)),
              ),
          },
          {
            onSuccess: () =>
              setUploading((prev) => prev.filter((u) => u.name !== file.name)),
            onError: () =>
              setUploading((prev) =>
                prev.map((u) =>
                  u.name === file.name ? { ...u, error: 'Upload failed' } : u,
                ),
              ),
          },
        );
      });
    },
    [productId, images.length, uploadMutation],
  );

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: ACCEPTED_TYPES,
    maxSize: MAX_SIZE,
    maxFiles: 5,
    disabled: images.length >= 5,
  });

  return (
    <div className="space-y-4">
      {/* Notice: image ops bypass the Save button */}
      <div className="flex items-start gap-2 rounded-md border border-amber-300 bg-amber-50 px-3 py-2 text-xs text-amber-900 dark:border-amber-800 dark:bg-amber-950 dark:text-amber-200">
        <Info className="mt-0.5 h-3.5 w-3.5 shrink-0" />
        <span>
          Image uploads and deletions are saved immediately and cannot be undone by leaving the page.
        </span>
      </div>

      {/* Existing images */}
      {images.length > 0 && (
        <div className="grid grid-cols-3 gap-3 sm:grid-cols-5">
          {images.map((img) => (
            <div key={img.id} className="group relative aspect-square">
              <img
                src={resolveUploadUrl(img.url)}
                alt={img.alt ?? ''}
                className="h-full w-full rounded-md object-cover"
              />
              {img.isPrimary && (
                <span className="absolute bottom-1 left-1 rounded-sm bg-primary px-1 text-[10px] text-primary-foreground">
                  Primary
                </span>
              )}
              <Button
                type="button"
                size="icon"
                variant="destructive"
                className="absolute right-1 top-1 h-6 w-6 opacity-0 transition-opacity group-hover:opacity-100"
                disabled={deleteMutation.isPending}
                onClick={() => setPendingDeleteId(img.id)}
              >
                <X className="h-3 w-3" />
              </Button>
            </div>
          ))}
        </div>
      )}

      {/* Uploading progress */}
      {uploading.map((u) => (
        <div key={u.name} className="rounded-md border p-3">
          <div className="flex items-center justify-between text-sm">
            <span className="truncate">{u.name}</span>
            {u.error ? (
              <span className="text-destructive">{u.error}</span>
            ) : (
              <span className="flex items-center gap-1 text-muted-foreground">
                <Loader2 className="h-3 w-3 animate-spin" />
                {u.progress}%
              </span>
            )}
          </div>
          {!u.error && (
            <div className="mt-1.5 h-1.5 overflow-hidden rounded-full bg-muted">
              <div
                className="h-full rounded-full bg-primary transition-all"
                style={{ width: `${u.progress}%` }}
              />
            </div>
          )}
        </div>
      ))}

      {/* Dropzone */}
      {images.length < 5 && (
        <div
          {...getRootProps()}
          className={cn(
            'flex cursor-pointer flex-col items-center justify-center gap-2 rounded-md border-2 border-dashed p-8 text-sm transition-colors',
            isDragActive
              ? 'border-primary bg-primary/5 text-primary'
              : 'border-muted-foreground/30 text-muted-foreground hover:border-primary/50',
          )}
        >
          <input {...getInputProps()} />
          <Upload className="h-6 w-6" />
          <p>
            {isDragActive
              ? 'Drop files here…'
              : 'Drag & drop images here, or click to browse'}
          </p>
          <p className="text-xs">JPG, PNG, WebP · max 5 MB · up to 5 images</p>
        </div>
      )}

      <ConfirmDialog
        open={pendingDeleteId !== null}
        title="Delete image?"
        description="This image will be removed immediately and cannot be recovered by clicking Back."
        confirmLabel="Delete"
        variant="destructive"
        loading={deleteMutation.isPending}
        onOpenChange={(open) => {
          if (!open) setPendingDeleteId(null);
        }}
        onConfirm={() => {
          if (!pendingDeleteId) return;
          deleteMutation.mutate(
            { productId, imageId: pendingDeleteId },
            { onSuccess: () => setPendingDeleteId(null) },
          );
        }}
      />
    </div>
  );
}
