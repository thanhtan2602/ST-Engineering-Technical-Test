using Catalog.Brands.Exceptions;

namespace Catalog.Brands.Features.DeleteBrand
{
    public record DeleteBrandCommand(Guid Id) : ICommand<DeleteBrandResult>;

    public record DeleteBrandResult(bool IsSuccess);

    public class DeleteBrandCommandValidator : AbstractValidator<DeleteBrandCommand>
    {
        public DeleteBrandCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class DeleteBrandCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<DeleteBrandCommand, DeleteBrandResult>
    {
        public async Task<DeleteBrandResult> Handle(DeleteBrandCommand command, CancellationToken cancellationToken)
        {
            var brand = await dbContext.Brands.FindAsync([command.Id], cancellationToken)
                ?? throw new BrandNotFoundException(command.Id);

            var productCount = await dbContext.Products.LongCountAsync(p => p.BrandId == command.Id, cancellationToken);
            if (productCount > 0)
                throw new Shared.Exceptions.BusinessRuleException(
                    $"Brand is used by {productCount} product(s) and cannot be deleted.",
                    new { productsUsing = productCount });

            brand.SoftDelete("system");
            await dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteBrandResult(true);
        }
    }
}
