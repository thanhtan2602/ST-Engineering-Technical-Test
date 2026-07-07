using Catalog.Brands.Exceptions;
using Catalog.Categories.Exceptions;
using Catalog.Data.Repositories;
using Catalog.Products.Dtos;
using Catalog.Products.Services;
using Catalog.ProductTypes.Exceptions;

namespace Catalog.Products.Features.UpdateProduct
{
    public record UpdateProductCommand(
        Guid Id,
        string Name,
        string Slug,
        string? Description,
        decimal Price,
        Guid CategoryId,
        Guid BrandId,
        ProductStatus Status,
        List<ProductAttributeInput> Attributes,
        uint? RowVersion = null) : ICommand<UpdateProductResult>;

    public record UpdateProductResult(bool IsSuccess);

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Slug).NotEmpty().MaximumLength(220)
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("Slug must be lowercase, alphanumeric, dash-separated.");
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.BrandId).NotEmpty();
            RuleFor(x => x.Status).IsInEnum();
        }
    }

    public class UpdateProductCommandHandler(
        IProductRepository productRepository,
        CatalogDbContext dbContext)
        : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            // Tracked load through aggregate root — xmin gets bumped on any change.
            var product = await productRepository.GetProductForUpdateAsync(command.Id, cancellationToken)
                ?? throw new ProductNotFoundException(command.Id);

            if (await productRepository.ExistsBySlugAsync(command.Slug, command.Id, cancellationToken))
                throw new Shared.Exceptions.BadRequestException($"Product with slug '{command.Slug}' already exists.");

            if (!await dbContext.Categories.AnyAsync(c => c.Id == command.CategoryId, cancellationToken))
                throw new CategoryNotFoundException(command.CategoryId);
            if (!await dbContext.Brands.AnyAsync(b => b.Id == command.BrandId, cancellationToken))
                throw new BrandNotFoundException(command.BrandId);

            var productType = await dbContext.ProductTypes
                .Include(pt => pt.TypeAttributes)
                    .ThenInclude(ta => ta.AttributeDefinition)
                .FirstOrDefaultAsync(pt => pt.Id == product.ProductTypeId, cancellationToken)
                ?? throw new ProductTypeNotFoundException(product.ProductTypeId);

            ProductAttributeValidator.Validate(productType, command.Attributes);

            // If client sent If-Match, override the OriginalValue so EF generates
            // WHERE xmin = <clientValue> — throws DbUpdateConcurrencyException on mismatch.
            if (command.RowVersion.HasValue)
                dbContext.Entry(product).Property("xmin").OriginalValue = command.RowVersion.Value;

            product.Update(command.Name, command.Slug, command.Description, command.Price, command.CategoryId, command.BrandId);
            product.ChangeStatus(command.Status);

            // Reconcile attributes: upsert incoming, remove ones no longer present.
            var incomingIds = command.Attributes.Select(a => a.AttributeDefinitionId).ToHashSet();
            foreach (var existing in product.Attributes.ToList())
            {
                if (!incomingIds.Contains(existing.AttributeDefinitionId))
                    product.RemoveAttribute(existing.AttributeDefinitionId);
            }
            foreach (var attr in command.Attributes)
                product.SetAttribute(attr.AttributeDefinitionId, attr.Value);

            await productRepository.SaveChangesAsync(product.Id, cancellationToken);

            return new UpdateProductResult(true);
        }
    }
}
