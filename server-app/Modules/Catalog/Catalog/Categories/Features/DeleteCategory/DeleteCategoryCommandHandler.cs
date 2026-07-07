using Catalog.Categories.Exceptions;

namespace Catalog.Categories.Features.DeleteCategory
{
    public record DeleteCategoryCommand(Guid Id) : ICommand<DeleteCategoryResult>;

    public record DeleteCategoryResult(bool IsSuccess);

    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class DeleteCategoryCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<DeleteCategoryCommand, DeleteCategoryResult>
    {
        public async Task<DeleteCategoryResult> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            var category = await dbContext.Categories.FindAsync([command.Id], cancellationToken)
                ?? throw new CategoryNotFoundException(command.Id);

            var hasChildren = await dbContext.Categories.AnyAsync(c => c.ParentId == command.Id, cancellationToken);
            if (hasChildren)
                throw new Shared.Exceptions.BadRequestException("Category has sub-categories and cannot be deleted.");

            var productCount = await dbContext.Products.LongCountAsync(p => p.CategoryId == command.Id, cancellationToken);
            if (productCount > 0)
                throw new Shared.Exceptions.BusinessRuleException(
                    $"Category is used by {productCount} product(s) and cannot be deleted.",
                    new { productsUsing = productCount });

            category.SoftDelete("system");
            await dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteCategoryResult(true);
        }
    }
}
