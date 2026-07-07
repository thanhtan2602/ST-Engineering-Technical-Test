using Catalog.Data.Repositories;
using Catalog.Products.Dtos;

namespace Catalog.Products.Features.UploadProductImage
{
    public record UploadProductImageCommand(
        Guid ProductId,
        IFormFile File,
        string? Alt,
        int DisplayOrder,
        bool IsPrimary,
        string WebRootPath) : ICommand<UploadProductImageResult>;

    public record UploadProductImageResult(ProductImageDto Image);

    public class UploadProductImageCommandHandler(IProductRepository productRepository)
        : ICommandHandler<UploadProductImageCommand, UploadProductImageResult>
    {
        private static readonly HashSet<string> AllowedMimeTypes =
            ["image/jpeg", "image/png", "image/webp"];

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public async Task<UploadProductImageResult> Handle(UploadProductImageCommand command, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetProductForUpdateAsync(command.ProductId, cancellationToken)
                ?? throw new ProductNotFoundException(command.ProductId);

            if (command.File.Length == 0)
                throw new Shared.Exceptions.BadRequestException("File is empty.");
            if (command.File.Length > MaxFileSizeBytes)
                throw new Shared.Exceptions.BadRequestException("File exceeds 5 MB limit.");
            if (!AllowedMimeTypes.Contains(command.File.ContentType.ToLower()))
                throw new Shared.Exceptions.BadRequestException(
                    "Unsupported file type. Allowed: image/jpeg, image/png, image/webp.");

            var ext = Path.GetExtension(command.File.FileName).ToLower();
            var fileName = $"{Guid.NewGuid()}{ext}";
            var folder = Path.Combine(command.WebRootPath, "uploads", "products", command.ProductId.ToString());
            Directory.CreateDirectory(folder);

            var fullPath = Path.Combine(folder, fileName);
            await using (var stream = File.Create(fullPath))
                await command.File.CopyToAsync(stream, cancellationToken);

            var imageUrl = $"/uploads/products/{command.ProductId}/{fileName}";
            var imageId = Guid.NewGuid();

            product.AddImage(imageId, imageUrl, command.Alt, command.DisplayOrder, command.IsPrimary);
            await productRepository.SaveChangesAsync(product.Id, cancellationToken);

            return new UploadProductImageResult(
                new ProductImageDto(imageId, imageUrl, command.Alt, command.DisplayOrder, command.IsPrimary));
        }
    }
}
