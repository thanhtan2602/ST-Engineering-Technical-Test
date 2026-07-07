using Catalog.Brands.Exceptions;

namespace Catalog.Brands.Features.UpdateBrand
{
    public record UpdateBrandCommand(Guid Id, string Name, string Slug) : ICommand<UpdateBrandResult>;

    public record UpdateBrandResult(bool IsSuccess);

    public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
    {
        public UpdateBrandCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Slug).NotEmpty().MaximumLength(180)
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("Slug must be lowercase, alphanumeric, dash-separated.");
        }
    }

    public class UpdateBrandCommandHandler(CatalogDbContext dbContext)
        : ICommandHandler<UpdateBrandCommand, UpdateBrandResult>
    {
        public async Task<UpdateBrandResult> Handle(UpdateBrandCommand command, CancellationToken cancellationToken)
        {
            var brand = await dbContext.Brands.FindAsync([command.Id], cancellationToken)
                ?? throw new BrandNotFoundException(command.Id);

            var slug = command.Slug.ToLower();
            if (await dbContext.Brands.AnyAsync(b => b.Id != command.Id && b.Slug == slug, cancellationToken))
                throw new Shared.Exceptions.BadRequestException($"Brand with slug '{command.Slug}' already exists.");

            brand.Update(command.Name, command.Slug);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateBrandResult(true);
        }
    }
}
