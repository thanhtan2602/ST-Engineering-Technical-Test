using Catalog.ProductTypes.Exceptions;

namespace Catalog.ProductTypes.Features.DetachAttribute
{
    public record DetachAttributeCommand(Guid ProductTypeId, Guid AttributeDefinitionId)
        : ICommand<DetachAttributeResult>;

    public record DetachAttributeResult(bool IsSuccess);

    public class DetachAttributeCommandValidator : AbstractValidator<DetachAttributeCommand>
    {
        public DetachAttributeCommandValidator()
        {
            RuleFor(x => x.ProductTypeId).NotEmpty();
            RuleFor(x => x.AttributeDefinitionId).NotEmpty();
        }
    }

    public class DetachAttributeCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<DetachAttributeCommand, DetachAttributeResult>
    {
        public async Task<DetachAttributeResult> Handle(DetachAttributeCommand command, CancellationToken cancellationToken)
        {
            var productType = await dbContext.ProductTypes
                .Include(x => x.TypeAttributes)
                .FirstOrDefaultAsync(x => x.Id == command.ProductTypeId, cancellationToken)
                ?? throw new ProductTypeNotFoundException(command.ProductTypeId);

            productType.DetachAttribute(command.AttributeDefinitionId);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new DetachAttributeResult(true);
        }
    }
}
