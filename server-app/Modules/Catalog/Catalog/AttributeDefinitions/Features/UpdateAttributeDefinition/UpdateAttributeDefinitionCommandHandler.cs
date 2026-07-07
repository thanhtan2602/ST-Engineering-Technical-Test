using Catalog.AttributeDefinitions.Exceptions;

namespace Catalog.AttributeDefinitions.Features.UpdateAttributeDefinition
{
    public record UpdateAttributeDefinitionCommand(
        Guid Id,
        string Label,
        string? Unit,
        List<string>? AllowedValues) : ICommand<UpdateAttributeDefinitionResult>;

    public record UpdateAttributeDefinitionResult(bool IsSuccess);

    public class UpdateAttributeDefinitionCommandValidator : AbstractValidator<UpdateAttributeDefinitionCommand>
    {
        public UpdateAttributeDefinitionCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Label).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Unit).MaximumLength(32);
        }
    }

    public class UpdateAttributeDefinitionCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<UpdateAttributeDefinitionCommand, UpdateAttributeDefinitionResult>
    {
        public async Task<UpdateAttributeDefinitionResult> Handle(UpdateAttributeDefinitionCommand command, CancellationToken cancellationToken)
        {
            var attr = await dbContext.AttributeDefinitions.FindAsync([command.Id], cancellationToken)
                ?? throw new AttributeDefinitionNotFoundException(command.Id);

            attr.Update(command.Label, command.Unit, command.AllowedValues);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateAttributeDefinitionResult(true);
        }
    }
}
