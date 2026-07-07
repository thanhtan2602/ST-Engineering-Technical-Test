namespace Catalog.Products.Features.RemoveProductAttribute
{
    public record RemoveProductAttributeResponse(bool IsSuccess);

    public class RemoveProductAttributeEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/products/{id:guid}/attributes/{attributeDefinitionId:guid}",
                async (Guid id, Guid attributeDefinitionId, ISender sender) =>
                {
                    var result = await sender.Send(new RemoveProductAttributeCommand(id, attributeDefinitionId));
                    return Results.Ok(result.Adapt<RemoveProductAttributeResponse>());
                })
            .WithName("RemoveProductAttribute")
            .WithTags("Products")
            .Produces<RemoveProductAttributeResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
