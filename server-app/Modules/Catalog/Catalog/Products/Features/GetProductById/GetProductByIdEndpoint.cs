using Catalog.Products.Dtos;

namespace Catalog.Products.Features.GetProductById
{
    public record GetProductByIdResponse(ProductDto Product);

    public class GetProductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/{id:guid}", async (Guid id, HttpContext http, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByIdQuery(id));
                http.Response.Headers.ETag = $"\"{result.Product.RowVersion}\"";
                var response = result.Adapt<GetProductByIdResponse>();
                return Results.Ok(response);
            })
            .WithName("GetProductById")
            .WithTags("Products")
            .Produces<GetProductByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
