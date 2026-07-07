using Catalog.Products.Dtos;

namespace Catalog.Products.Features.CreateProduct
{
    public record CreateProductRequest(
        string Sku,
        string Name,
        string Slug,
        string? Description,
        decimal Price,
        Guid CategoryId,
        Guid BrandId,
        Guid ProductTypeId,
        ProductStatus Status,
        List<ProductAttributeInput>? Attributes,
        List<ProductImageInput>? Images);

    public record CreateProductResponse(Guid Id);

    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (CreateProductRequest request, ISender sender) =>
            {
                var command = new CreateProductCommand(
                    request.Sku,
                    request.Name,
                    request.Slug,
                    request.Description,
                    request.Price,
                    request.CategoryId,
                    request.BrandId,
                    request.ProductTypeId,
                    request.Status,
                    request.Attributes ?? new(),
                    request.Images ?? new());
                var result = await sender.Send(command);
                var response = result.Adapt<CreateProductResponse>();
                return Results.Created($"/products/{response.Id}", response);
            })
            .WithName("CreateProduct")
            .WithTags("Products")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
