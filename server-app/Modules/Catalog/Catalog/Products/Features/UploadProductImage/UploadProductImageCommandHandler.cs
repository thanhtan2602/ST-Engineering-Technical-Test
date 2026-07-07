using Catalog.Data.Repositories;
using Catalog.Products.Dtos;
using Shared.Exceptions;

namespace Catalog.Products.Features.UploadProductImage
{
    public record UploadProductImageCommand(
        Guid ProductId,
        IFormFile File,
        string? Alt,
        int DisplayOrder,
        string WebRootPath) : ICommand<UploadProductImageResult>;

    public record UploadProductImageResult(ProductImageDto Image);

    public class UploadProductImageCommandHandler(
        IProductRepository productRepository,
        CatalogDbContext dbContext)
        : ICommandHandler<UploadProductImageCommand, UploadProductImageResult>
    {
        private static readonly HashSet<string> AllowedMimeTypes =
            ["image/jpeg", "image/png", "image/webp"];

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public async Task<UploadProductImageResult> Handle(UploadProductImageCommand command, CancellationToken cancellationToken)
        {
            // Verify product exists without loading it into the change tracker.
            // Loading via GetProductForUpdateAsync would track xmin and generate an
            // unnecessary UPDATE on the Products row, triggering a concurrency conflict.
            if (!await dbContext.Products.AnyAsync(p => p.Id == command.ProductId, cancellationToken))
                throw new ProductNotFoundException(command.ProductId);

            if (command.File.Length == 0)
                throw new BadRequestException("File is empty.");
            if (command.File.Length > MaxFileSizeBytes)
                throw new BadRequestException("File exceeds 5 MB limit.");
            if (!AllowedMimeTypes.Contains(command.File.ContentType.ToLower()))
                throw new BadRequestException(
                    "Unsupported file type. Allowed: image/jpeg, image/png, image/webp.");

            var ext = Path.GetExtension(command.File.FileName).ToLower();
            var fileName = $"{Guid.NewGuid()}{ext}";
            var folder = Path.Combine(command.WebRootPath, "uploads", "products", command.ProductId.ToString());
            Directory.CreateDirectory(folder);

            var fullPath = Path.Combine(folder, fileName);
            await using (var stream = System.IO.File.Create(fullPath))
                await command.File.CopyToAsync(stream, cancellationToken);

            var imageUrl = $"/uploads/products/{command.ProductId}/{fileName}";
            var imageId = Guid.NewGuid();

            // Every new upload becomes the primary image — demote any existing primary.
            var currentPrimary = await dbContext.Set<ProductImage>()
                .Where(i => i.ProductId == command.ProductId && i.IsPrimary)
                .ToListAsync(cancellationToken);
            foreach (var img in currentPrimary)
                img.SetPrimary(false);

            dbContext.Add(ProductImage.Create(imageId, command.ProductId, imageUrl, command.Alt, command.DisplayOrder, isPrimary: true));
            await productRepository.SaveChangesAsync(command.ProductId, cancellationToken);

            return new UploadProductImageResult(
                new ProductImageDto(imageId, imageUrl, command.Alt, command.DisplayOrder, IsPrimary: true));
        }
    }
}
