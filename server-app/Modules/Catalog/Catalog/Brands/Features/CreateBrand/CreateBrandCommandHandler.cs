using Catalog.Brands.Exceptions;

namespace Catalog.Brands.Features.CreateBrand
{
    public record CreateBrandCommand(string Name, string Slug) : ICommand<CreateBrandResult>;

    public record CreateBrandResult(Guid Id);

    public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
    {
        public CreateBrandCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Slug).NotEmpty().MaximumLength(180)
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("Slug must be lowercase, alphanumeric, dash-separated.");
        }
    }

    public class CreateBrandCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<CreateBrandCommand, CreateBrandResult>
    {
        public async Task<CreateBrandResult> Handle(CreateBrandCommand command, CancellationToken cancellationToken)
        {
            if (await dbContext.Brands.AnyAsync(b => b.Slug == command.Slug.ToLower(), cancellationToken))
                throw new Shared.Exceptions.BadRequestException($"Brand with slug '{command.Slug}' already exists.");

            var brand = Brand.Create(Guid.NewGuid(), command.Name, command.Slug);
            dbContext.Brands.Add(brand);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateBrandResult(brand.Id);
        }
    }
}
