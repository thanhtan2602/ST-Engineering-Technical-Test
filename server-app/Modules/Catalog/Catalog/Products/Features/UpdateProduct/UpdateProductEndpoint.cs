using Catalog.Products.Dtos;

namespace Catalog.Products.Features.UpdateProduct
{
    public record UpdateProductRequest(
        string Name,
        string Slug,
        string? Description,
        decimal Price,
        Guid CategoryId,
        Guid BrandId,
        ProductStatus Status,
        List<ProductAttributeInput>? Attributes);

    public record UpdateProductResponse(bool IsSuccess);

    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products/{id:guid}", async (Guid id, UpdateProductRequest request, HttpContext http, ISender sender) =>
            {
                uint? rowVersion = null;
                var ifMatch = http.Request.Headers.IfMatch.FirstOrDefault();
                if (ifMatch is not null && uint.TryParse(ifMatch.Trim('"'), out var parsed))
                    rowVersion = parsed;

                var command = new UpdateProductCommand(
                    id,
                    request.Name,
                    request.Slug,
                    request.Description,
                    request.Price,
                    request.CategoryId,
                    request.BrandId,
                    request.Status,
                    request.Attributes ?? new(),
                    rowVersion);
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateProductResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateProduct")
            .WithTags("Products")
            .Produces<UpdateProductResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
        }
    }
}
