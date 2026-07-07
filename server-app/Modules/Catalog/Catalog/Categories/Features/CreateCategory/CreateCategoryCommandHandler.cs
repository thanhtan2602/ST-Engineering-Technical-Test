using Catalog.Categories.Exceptions;

namespace Catalog.Categories.Features.CreateCategory
{
    public record CreateCategoryCommand(string Name, string Slug, Guid? ParentId)
        : ICommand<CreateCategoryResult>;

    public record CreateCategoryResult(Guid Id);

    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Slug).NotEmpty().MaximumLength(180)
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("Slug must be lowercase, alphanumeric, dash-separated.");
        }
    }

    public class CreateCategoryCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<CreateCategoryCommand, CreateCategoryResult>
    {
        public async Task<CreateCategoryResult> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var slug = command.Slug.ToLower();
            if (await dbContext.Categories.AnyAsync(c => c.Slug == slug, cancellationToken))
                throw new Shared.Exceptions.BadRequestException($"Category with slug '{command.Slug}' already exists.");

            if (command.ParentId.HasValue)
            {
                var parentExists = await dbContext.Categories
                    .AnyAsync(c => c.Id == command.ParentId.Value, cancellationToken);
                if (!parentExists)
                    throw new CategoryNotFoundException(command.ParentId.Value);
            }

            var category = Category.Create(Guid.NewGuid(), command.Name, command.Slug, command.ParentId);
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateCategoryResult(category.Id);
        }
    }
}
