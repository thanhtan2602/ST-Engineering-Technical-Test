using Catalog.ProductTypes.Exceptions;

namespace Catalog.ProductTypes.Features.DeleteProductType
{
    public record DeleteProductTypeCommand(Guid Id) : ICommand<DeleteProductTypeResult>;

    public record DeleteProductTypeResult(bool IsSuccess);

    public class DeleteProductTypeCommandValidator : AbstractValidator<DeleteProductTypeCommand>
    {
        public DeleteProductTypeCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class DeleteProductTypeCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<DeleteProductTypeCommand, DeleteProductTypeResult>
    {
        public async Task<DeleteProductTypeResult> Handle(DeleteProductTypeCommand command, CancellationToken cancellationToken)
        {
            var productType = await dbContext.ProductTypes.FindAsync([command.Id], cancellationToken)
                ?? throw new ProductTypeNotFoundException(command.Id);

            var hasProducts = await dbContext.Products.AnyAsync(p => p.ProductTypeId == command.Id, cancellationToken);
            if (hasProducts)
                throw new Shared.Exceptions.BadRequestException("ProductType is in use by products and cannot be deleted.");

            productType.SoftDelete("system");
            await dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteProductTypeResult(true);
        }
    }
}
