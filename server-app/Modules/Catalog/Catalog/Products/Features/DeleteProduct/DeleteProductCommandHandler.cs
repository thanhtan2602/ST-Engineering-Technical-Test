using Catalog.Data.Repositories;

namespace Catalog.Products.Features.DeleteProduct
{
    public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;

    public record DeleteProductResult(bool IsSuccess);

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class DeleteProductCommandHandler(IProductRepository productRepository)
        : ICommandHandler<DeleteProductCommand, DeleteProductResult>
    {
        public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetProductForUpdateAsync(command.Id, cancellationToken)
                ?? throw new ProductNotFoundException(command.Id);

            product.SoftDelete("system");
            await productRepository.SaveChangesAsync(product.Id, cancellationToken);

            return new DeleteProductResult(true);
        }
    }
}
