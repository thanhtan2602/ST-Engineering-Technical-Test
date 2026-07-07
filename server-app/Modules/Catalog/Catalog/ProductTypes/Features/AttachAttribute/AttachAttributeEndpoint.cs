namespace Catalog.ProductTypes.Features.AttachAttribute
{
    public record AttachAttributeRequest(Guid AttributeDefinitionId, bool IsRequired, int DisplayOrder);
    public record AttachAttributeResponse(bool IsSuccess);

    public class AttachAttributeEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/product-types/{id:guid}/attributes", async (Guid id, AttachAttributeRequest request, ISender sender) =>
            {
                var command = new AttachAttributeCommand(id, request.AttributeDefinitionId, request.IsRequired, request.DisplayOrder);
                var result = await sender.Send(command);
                var response = result.Adapt<AttachAttributeResponse>();
                return Results.Ok(response);
            })
            .WithName("AttachAttribute")
            .WithTags("ProductTypes")
            .Produces<AttachAttributeResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
