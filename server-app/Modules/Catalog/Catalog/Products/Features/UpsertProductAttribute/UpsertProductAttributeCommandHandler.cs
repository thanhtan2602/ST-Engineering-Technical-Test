using Catalog.Data.Repositories;
using Catalog.Products.Dtos;
using Catalog.Products.Services;
using Catalog.ProductTypes.Exceptions;

namespace Catalog.Products.Features.UpsertProductAttribute
{
    public record UpsertProductAttributeCommand(Guid ProductId, ProductAttributeInput Attribute)
        : ICommand<UpsertProductAttributeResult>;

    public record UpsertProductAttributeResult(bool IsSuccess);

    public class UpsertProductAttributeCommandValidator : AbstractValidator<UpsertProductAttributeCommand>
    {
        public UpsertProductAttributeCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Attribute.AttributeDefinitionId).NotEmpty();
            RuleFor(x => x.Attribute.Value).NotEmpty();
        }
    }

    public class UpsertProductAttributeCommandHandler(
        IProductRepository productRepository,
        CatalogDbContext dbContext)
        : ICommandHandler<UpsertProductAttributeCommand, UpsertProductAttributeResult>
    {
        public async Task<UpsertProductAttributeResult> Handle(UpsertProductAttributeCommand command, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetProductForUpdateAsync(command.ProductId, cancellationToken)
                ?? throw new ProductNotFoundException(command.ProductId);

            var productType = await dbContext.ProductTypes
                .Include(pt => pt.TypeAttributes)
                    .ThenInclude(ta => ta.AttributeDefinition)
                .FirstOrDefaultAsync(pt => pt.Id == product.ProductTypeId, cancellationToken)
                ?? throw new ProductTypeNotFoundException(product.ProductTypeId);

            // Validate just this one attribute (whitelist + DataType only — no required check).
            ProductAttributeValidator.ValidateSingle(productType, command.Attribute);

            product.SetAttribute(command.Attribute.AttributeDefinitionId, command.Attribute.Value);
            await productRepository.SaveChangesAsync(product.Id, cancellationToken);

            return new UpsertProductAttributeResult(true);
        }
    }
}
