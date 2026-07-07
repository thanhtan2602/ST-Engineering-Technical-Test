using Catalog.ProductTypes.Exceptions;

namespace Catalog.ProductTypes.Features.UpdateProductType
{
    public record UpdateProductTypeCommand(Guid Id, string Name) : ICommand<UpdateProductTypeResult>;

    public record UpdateProductTypeResult(bool IsSuccess);

    public class UpdateProductTypeCommandValidator : AbstractValidator<UpdateProductTypeCommand>
    {
        public UpdateProductTypeCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        }
    }

    public class UpdateProductTypeCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<UpdateProductTypeCommand, UpdateProductTypeResult>
    {
        public async Task<UpdateProductTypeResult> Handle(UpdateProductTypeCommand command, CancellationToken cancellationToken)
        {
            var productType = await dbContext.ProductTypes.FindAsync([command.Id], cancellationToken)
                ?? throw new ProductTypeNotFoundException(command.Id);

            productType.Update(command.Name);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateProductTypeResult(true);
        }
    }
}
