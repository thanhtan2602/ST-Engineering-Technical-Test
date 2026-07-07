using Catalog.Categories.Exceptions;

namespace Catalog.Categories.Features.UpdateCategory
{
    public record UpdateCategoryCommand(Guid Id, string Name, string Slug, Guid? ParentId)
        : ICommand<UpdateCategoryResult>;

    public record UpdateCategoryResult(bool IsSuccess);

    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Slug).NotEmpty().MaximumLength(180)
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("Slug must be lowercase, alphanumeric, dash-separated.");
        }
    }

    public class UpdateCategoryCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<UpdateCategoryCommand, UpdateCategoryResult>
    {
        public async Task<UpdateCategoryResult> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            var category = await dbContext.Categories.FindAsync([command.Id], cancellationToken)
                ?? throw new CategoryNotFoundException(command.Id);

            var slug = command.Slug.ToLower();
            if (await dbContext.Categories.AnyAsync(c => c.Id != command.Id && c.Slug == slug, cancellationToken))
                throw new Shared.Exceptions.BadRequestException($"Category with slug '{command.Slug}' already exists.");

            if (command.ParentId.HasValue)
            {
                if (command.ParentId.Value == command.Id)
                    throw new Shared.Exceptions.BadRequestException("Category cannot be its own parent.");

                var parentExists = await dbContext.Categories
                    .AnyAsync(c => c.Id == command.ParentId.Value, cancellationToken);
                if (!parentExists)
                    throw new CategoryNotFoundException(command.ParentId.Value);

                if (await WouldCreateCycleAsync(dbContext, command.Id, command.ParentId.Value, cancellationToken))
                    throw new Shared.Exceptions.BadRequestException("ParentId would create a cycle in the category hierarchy.");
            }

            category.Update(command.Name, command.Slug, command.ParentId);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateCategoryResult(true);
        }

        private static async Task<bool> WouldCreateCycleAsync(
            CatalogDbContext dbContext, Guid categoryId, Guid newParentId, CancellationToken ct)
        {
            var current = newParentId;
            while (true)
            {
                if (current == categoryId) return true;
                var parent = await dbContext.Categories
                    .Where(c => c.Id == current)
                    .Select(c => c.ParentId)
                    .FirstOrDefaultAsync(ct);
                if (parent is null) return false;
                current = parent.Value;
            }
        }
    }
}
