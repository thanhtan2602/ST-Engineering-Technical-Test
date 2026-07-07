using Catalog.AttributeDefinitions.Exceptions;

namespace Catalog.AttributeDefinitions.Features.DeleteAttributeDefinition
{
    public record DeleteAttributeDefinitionCommand(Guid Id) : ICommand<DeleteAttributeDefinitionResult>;

    public record DeleteAttributeDefinitionResult(bool IsSuccess);

    public class DeleteAttributeDefinitionCommandValidator : AbstractValidator<DeleteAttributeDefinitionCommand>
    {
        public DeleteAttributeDefinitionCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class DeleteAttributeDefinitionCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<DeleteAttributeDefinitionCommand, DeleteAttributeDefinitionResult>
    {
        public async Task<DeleteAttributeDefinitionResult> Handle(DeleteAttributeDefinitionCommand command, CancellationToken cancellationToken)
        {
            var attr = await dbContext.AttributeDefinitions.FindAsync([command.Id], cancellationToken)
                ?? throw new AttributeDefinitionNotFoundException(command.Id);

            // Prevent delete if referenced by any ProductType or ProductAttribute
            var referencedByType = await dbContext.ProductTypeAttributes
                .AnyAsync(x => x.AttributeDefinitionId == command.Id, cancellationToken);
            var referencedByProduct = await dbContext.ProductAttributes
                .AnyAsync(x => x.AttributeDefinitionId == command.Id, cancellationToken);

            if (referencedByType || referencedByProduct)
                throw new Shared.Exceptions.BadRequestException(
                    "AttributeDefinition is in use and cannot be deleted.",
                    $"Referenced by ProductType={referencedByType}, ProductAttribute={referencedByProduct}");

            dbContext.AttributeDefinitions.Remove(attr);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteAttributeDefinitionResult(true);
        }
    }
}
