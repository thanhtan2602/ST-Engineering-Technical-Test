using Catalog.Data.Repositories;
using Shared.Exceptions;

namespace Catalog.Products.Features.DeleteProductImage
{
    public record DeleteProductImageCommand(Guid ProductId, Guid ImageId)
        : ICommand<DeleteProductImageResult>;

    public record DeleteProductImageResult(bool IsSuccess);

    public class DeleteProductImageCommandValidator : AbstractValidator<DeleteProductImageCommand>
    {
        public DeleteProductImageCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.ImageId).NotEmpty();
        }
    }

    public class DeleteProductImageCommandHandler(
        IProductRepository productRepository,
        CatalogDbContext dbContext,
        IWebHostEnvironment env)
        : ICommandHandler<DeleteProductImageCommand, DeleteProductImageResult>
    {
        public async Task<DeleteProductImageResult> Handle(DeleteProductImageCommand command, CancellationToken cancellationToken)
        {
            // Load the image directly — avoids tracking the Product entity with xmin,
            // which would otherwise generate a spurious UPDATE on the Products row.
            var image = await dbContext.Set<ProductImage>()
                .FirstOrDefaultAsync(i => i.ProductId == command.ProductId && i.Id == command.ImageId, cancellationToken);

            if (image is null)
            {
                // Distinguish between product-not-found and image-not-found.
                if (!await dbContext.Products.AnyAsync(p => p.Id == command.ProductId, cancellationToken))
                    throw new ProductNotFoundException(command.ProductId);
                throw new NotFoundException("ProductImage", command.ImageId);
            }

            var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
            var physicalPath = Path.Combine(webRoot, image.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(physicalPath))
                System.IO.File.Delete(physicalPath);

            dbContext.Set<ProductImage>().Remove(image);
            await productRepository.SaveChangesAsync(command.ProductId, cancellationToken);

            return new DeleteProductImageResult(true);
        }
    }
}
