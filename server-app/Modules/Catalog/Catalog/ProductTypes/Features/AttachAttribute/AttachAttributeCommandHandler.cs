using Catalog.AttributeDefinitions.Exceptions;
using Catalog.ProductTypes.Exceptions;

namespace Catalog.ProductTypes.Features.AttachAttribute
{
    public record AttachAttributeCommand(Guid ProductTypeId, Guid AttributeDefinitionId, bool IsRequired, int DisplayOrder)
        : ICommand<AttachAttributeResult>;

    public record AttachAttributeResult(bool IsSuccess);

    public class AttachAttributeCommandValidator : AbstractValidator<AttachAttributeCommand>
    {
        public AttachAttributeCommandValidator()
        {
            RuleFor(x => x.ProductTypeId).NotEmpty();
            RuleFor(x => x.AttributeDefinitionId).NotEmpty();
            RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        }
    }

    public class AttachAttributeCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<AttachAttributeCommand, AttachAttributeResult>
    {
        public async Task<AttachAttributeResult> Handle(AttachAttributeCommand command, CancellationToken cancellationToken)
        {
            var productType = await dbContext.ProductTypes
                .Include(x => x.TypeAttributes)
                .FirstOrDefaultAsync(x => x.Id == command.ProductTypeId, cancellationToken)
                ?? throw new ProductTypeNotFoundException(command.ProductTypeId);

            var attrExists = await dbContext.AttributeDefinitions
                .AnyAsync(a => a.Id == command.AttributeDefinitionId, cancellationToken);
            if (!attrExists)
                throw new AttributeDefinitionNotFoundException(command.AttributeDefinitionId);

            productType.AttachAttribute(command.AttributeDefinitionId, command.IsRequired, command.DisplayOrder);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new AttachAttributeResult(true);
        }
    }
}
