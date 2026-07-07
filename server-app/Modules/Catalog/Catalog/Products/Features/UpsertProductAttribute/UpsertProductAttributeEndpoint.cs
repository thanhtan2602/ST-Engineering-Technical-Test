using Catalog.Products.Dtos;

namespace Catalog.Products.Features.UpsertProductAttribute
{
    public record UpsertProductAttributeRequest(Guid AttributeDefinitionId, string Value);
    public record UpsertProductAttributeResponse(bool IsSuccess);

    public class UpsertProductAttributeEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products/{id:guid}/attributes", async (Guid id, UpsertProductAttributeRequest request, ISender sender) =>
            {
                var command = new UpsertProductAttributeCommand(id,
                    new ProductAttributeInput(request.AttributeDefinitionId, request.Value));
                var result = await sender.Send(command);
                return Results.Ok(result.Adapt<UpsertProductAttributeResponse>());
            })
            .WithName("UpsertProductAttribute")
            .WithTags("Products")
            .Produces<UpsertProductAttributeResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
