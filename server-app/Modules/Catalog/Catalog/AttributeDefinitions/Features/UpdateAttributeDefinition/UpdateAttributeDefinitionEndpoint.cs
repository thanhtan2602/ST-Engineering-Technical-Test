namespace Catalog.AttributeDefinitions.Features.UpdateAttributeDefinition
{
    public record UpdateAttributeDefinitionRequest(string Label, string? Unit, List<string>? AllowedValues);
    public record UpdateAttributeDefinitionResponse(bool IsSuccess);

    public class UpdateAttributeDefinitionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/attribute-definitions/{id:guid}", async (Guid id, UpdateAttributeDefinitionRequest request, ISender sender) =>
            {
                var command = new UpdateAttributeDefinitionCommand(id, request.Label, request.Unit, request.AllowedValues);
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateAttributeDefinitionResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateAttributeDefinition")
            .WithTags("AttributeDefinitions")
            .Produces<UpdateAttributeDefinitionResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
