using Catalog.Data.Repositories;

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
        IWebHostEnvironment env)
        : ICommandHandler<DeleteProductImageCommand, DeleteProductImageResult>
    {
        public async Task<DeleteProductImageResult> Handle(DeleteProductImageCommand command, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetProductForUpdateAsync(command.ProductId, cancellationToken)
                ?? throw new ProductNotFoundException(command.ProductId);

            var image = product.Images.FirstOrDefault(i => i.Id == command.ImageId)
                ?? throw new Shared.Exceptions.NotFoundException("ProductImage", command.ImageId);

            // Best-effort delete from disk; don't fail if file is gone.
            var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
            var physicalPath = Path.Combine(webRoot, image.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(physicalPath))
                File.Delete(physicalPath);

            product.RemoveImage(command.ImageId);
            await productRepository.SaveChangesAsync(product.Id, cancellationToken);

            return new DeleteProductImageResult(true);
        }
    }
}
