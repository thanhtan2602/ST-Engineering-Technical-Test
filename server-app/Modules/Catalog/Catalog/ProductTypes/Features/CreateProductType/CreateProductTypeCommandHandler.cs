namespace Catalog.ProductTypes.Features.CreateProductType
{
    public record CreateProductTypeCommand(string Code, string Name) : ICommand<CreateProductTypeResult>;

    public record CreateProductTypeResult(Guid Id);

    public class CreateProductTypeCommandValidator : AbstractValidator<CreateProductTypeCommand>
    {
        public CreateProductTypeCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(64)
                .Matches("^[a-z0-9_]+$")
                .WithMessage("Code must be lowercase alphanumeric with underscores.");
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        }
    }

    public class CreateProductTypeCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<CreateProductTypeCommand, CreateProductTypeResult>
    {
        public async Task<CreateProductTypeResult> Handle(CreateProductTypeCommand command, CancellationToken cancellationToken)
        {
            var code = command.Code.ToLower();
            if (await dbContext.ProductTypes.AnyAsync(x => x.Code == code, cancellationToken))
                throw new Shared.Exceptions.BadRequestException($"ProductType with code '{command.Code}' already exists.");

            var productType = ProductType.Create(Guid.NewGuid(), command.Code, command.Name);
            dbContext.ProductTypes.Add(productType);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateProductTypeResult(productType.Id);
        }
    }
}
