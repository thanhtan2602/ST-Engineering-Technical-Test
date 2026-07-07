namespace Catalog.AttributeDefinitions.Features.DeleteAttributeDefinition
{
    public record DeleteAttributeDefinitionResponse(bool IsSuccess);

    public class DeleteAttributeDefinitionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/attribute-definitions/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteAttributeDefinitionCommand(id));
                var response = result.Adapt<DeleteAttributeDefinitionResponse>();
                return Results.Ok(response);
            })
            .WithName("DeleteAttributeDefinition")
            .WithTags("AttributeDefinitions")
            .Produces<DeleteAttributeDefinitionResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
