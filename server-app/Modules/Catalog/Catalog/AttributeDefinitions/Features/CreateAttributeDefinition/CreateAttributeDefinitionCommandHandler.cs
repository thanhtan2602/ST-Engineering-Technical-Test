namespace Catalog.AttributeDefinitions.Features.CreateAttributeDefinition
{
    public record CreateAttributeDefinitionCommand(
        string Key,
        string Label,
        AttributeDataType DataType,
        string? Unit,
        List<string>? AllowedValues) : ICommand<CreateAttributeDefinitionResult>;

    public record CreateAttributeDefinitionResult(Guid Id);

    public class CreateAttributeDefinitionCommandValidator : AbstractValidator<CreateAttributeDefinitionCommand>
    {
        public CreateAttributeDefinitionCommandValidator()
        {
            RuleFor(x => x.Key).NotEmpty().MaximumLength(64)
                .Matches("^[a-z0-9_]+$")
                .WithMessage("Key must be lowercase alphanumeric with underscores.");
            RuleFor(x => x.Label).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Unit).MaximumLength(32);
            RuleFor(x => x.DataType).IsInEnum();
            When(x => x.DataType == AttributeDataType.Enum, () =>
            {
                RuleFor(x => x.AllowedValues)
                    .NotNull().WithMessage("AllowedValues is required for Enum type.")
                    .Must(v => v != null && v.Count > 0)
                    .WithMessage("AllowedValues must contain at least one value.");
            });
        }
    }

    public class CreateAttributeDefinitionCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<CreateAttributeDefinitionCommand, CreateAttributeDefinitionResult>
    {
        public async Task<CreateAttributeDefinitionResult> Handle(CreateAttributeDefinitionCommand command, CancellationToken cancellationToken)
        {
            var key = command.Key.ToLower();
            if (await dbContext.AttributeDefinitions.AnyAsync(a => a.Key == key, cancellationToken))
                throw new Shared.Exceptions.BadRequestException($"AttributeDefinition with key '{command.Key}' already exists.");

            var attr = AttributeDefinition.Create(
                Guid.NewGuid(),
                command.Key,
                command.Label,
                command.DataType,
                command.Unit,
                command.AllowedValues);

            dbContext.AttributeDefinitions.Add(attr);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateAttributeDefinitionResult(attr.Id);
        }
    }
}
