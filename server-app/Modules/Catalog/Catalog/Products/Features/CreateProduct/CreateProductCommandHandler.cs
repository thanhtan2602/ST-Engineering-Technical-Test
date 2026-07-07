using Catalog.Brands.Exceptions;
using Catalog.Categories.Exceptions;
using Catalog.Data.Repositories;
using Catalog.Products.Dtos;
using Catalog.Products.Services;
using Catalog.ProductTypes.Exceptions;

namespace Catalog.Products.Features.CreateProduct
{
    public record CreateProductCommand(
        string Sku,
        string Name,
        string Slug,
        string? Description,
        decimal Price,
        Guid CategoryId,
        Guid BrandId,
        Guid ProductTypeId,
        ProductStatus Status,
        List<ProductAttributeInput> Attributes,
        List<ProductImageInput> Images) : ICommand<CreateProductResult>;

    public record CreateProductResult(Guid Id);

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Sku).NotEmpty().MaximumLength(64);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Slug).NotEmpty().MaximumLength(220)
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("Slug must be lowercase, alphanumeric, dash-separated.");
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.BrandId).NotEmpty();
            RuleFor(x => x.ProductTypeId).NotEmpty();
            RuleFor(x => x.Status).IsInEnum();
            RuleForEach(x => x.Images).ChildRules(img =>
            {
                img.RuleFor(i => i.Url).NotEmpty().MaximumLength(500);
                img.RuleFor(i => i.DisplayOrder).GreaterThanOrEqualTo(0);
            });
        }
    }

    public class CreateProductCommandHandler(
        IProductRepository productRepository,
        CatalogDbContext dbContext)
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            if (await productRepository.ExistsBySkuAsync(command.Sku, null, cancellationToken))
                throw new Shared.Exceptions.BadRequestException($"Product with SKU '{command.Sku}' already exists.");
            if (await productRepository.ExistsBySlugAsync(command.Slug, null, cancellationToken))
                throw new Shared.Exceptions.BadRequestException($"Product with slug '{command.Slug}' already exists.");

            // Reference-data existence checks — kept on DbContext (not in repository scope).
            if (!await dbContext.Categories.AnyAsync(c => c.Id == command.CategoryId, cancellationToken))
                throw new CategoryNotFoundException(command.CategoryId);
            if (!await dbContext.Brands.AnyAsync(b => b.Id == command.BrandId, cancellationToken))
                throw new BrandNotFoundException(command.BrandId);

            var productType = await dbContext.ProductTypes
                .Include(pt => pt.TypeAttributes)
                    .ThenInclude(ta => ta.AttributeDefinition)
                .FirstOrDefaultAsync(pt => pt.Id == command.ProductTypeId, cancellationToken)
                ?? throw new ProductTypeNotFoundException(command.ProductTypeId);

            ProductAttributeValidator.Validate(productType, command.Attributes);

            var product = Product.Create(
                Guid.NewGuid(),
                command.Sku,
                command.Name,
                command.Slug,
                command.Description,
                command.Price,
                command.CategoryId,
                command.BrandId,
                command.ProductTypeId,
                command.Status);

            foreach (var attr in command.Attributes)
                product.SetAttribute(attr.AttributeDefinitionId, attr.Value);

            foreach (var img in command.Images)
                product.AddImage(Guid.NewGuid(), img.Url, img.Alt, img.DisplayOrder, img.IsPrimary);

            await productRepository.AddAsync(product, cancellationToken);
            await productRepository.SaveChangesAsync(product.Id, cancellationToken);

            return new CreateProductResult(product.Id);
        }
    }
}
