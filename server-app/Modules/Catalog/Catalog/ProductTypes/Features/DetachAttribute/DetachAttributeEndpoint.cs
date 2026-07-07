namespace Catalog.ProductTypes.Features.DetachAttribute
{
    public record DetachAttributeResponse(bool IsSuccess);

    public class DetachAttributeEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/product-types/{id:guid}/attributes/{attributeDefinitionId:guid}", async (
                Guid id, Guid attributeDefinitionId, ISender sender) =>
            {
                var result = await sender.Send(new DetachAttributeCommand(id, attributeDefinitionId));
                var response = result.Adapt<DetachAttributeResponse>();
                return Results.Ok(response);
            })
            .WithName("DetachAttribute")
            .WithTags("ProductTypes")
            .Produces<DetachAttributeResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
