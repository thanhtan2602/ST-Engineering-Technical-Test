using Catalog.Data.Repositories;

namespace Catalog.Products.Features.RemoveProductAttribute
{
    public record RemoveProductAttributeCommand(Guid ProductId, Guid AttributeDefinitionId)
        : ICommand<RemoveProductAttributeResult>;

    public record RemoveProductAttributeResult(bool IsSuccess);

    public class RemoveProductAttributeCommandValidator : AbstractValidator<RemoveProductAttributeCommand>
    {
        public RemoveProductAttributeCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.AttributeDefinitionId).NotEmpty();
        }
    }

    public class RemoveProductAttributeCommandHandler(
        IProductRepository productRepository,
        CatalogDbContext dbContext)
        : ICommandHandler<RemoveProductAttributeCommand, RemoveProductAttributeResult>
    {
        public async Task<RemoveProductAttributeResult> Handle(RemoveProductAttributeCommand command, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetProductForUpdateAsync(command.ProductId, cancellationToken)
                ?? throw new ProductNotFoundException(command.ProductId);

            // Guard: cannot remove a required attribute.
            var productType = await dbContext.ProductTypes
                .Include(pt => pt.TypeAttributes)
                .FirstOrDefaultAsync(pt => pt.Id == product.ProductTypeId, cancellationToken);

            var isRequired = productType?.TypeAttributes
                .Any(ta => ta.AttributeDefinitionId == command.AttributeDefinitionId && ta.IsRequired) ?? false;

            if (isRequired)
                throw new Shared.Exceptions.BadRequestException(
                    "Cannot remove a required attribute. Update its value instead.");

            product.RemoveAttribute(command.AttributeDefinitionId);
            await productRepository.SaveChangesAsync(product.Id, cancellationToken);

            return new RemoveProductAttributeResult(true);
        }
    }
}
