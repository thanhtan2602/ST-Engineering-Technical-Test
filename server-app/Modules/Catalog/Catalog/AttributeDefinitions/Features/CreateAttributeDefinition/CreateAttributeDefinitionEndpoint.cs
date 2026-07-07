namespace Catalog.AttributeDefinitions.Features.CreateAttributeDefinition
{
    public record CreateAttributeDefinitionRequest(
        string Key,
        string Label,
        AttributeDataType DataType,
        string? Unit,
        List<string>? AllowedValues);

    public record CreateAttributeDefinitionResponse(Guid Id);

    public class CreateAttributeDefinitionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/attribute-definitions", async (CreateAttributeDefinitionRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateAttributeDefinitionCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateAttributeDefinitionResponse>();
                return Results.Created($"/attribute-definitions/{response.Id}", response);
            })
            .WithName("CreateAttributeDefinition")
            .WithTags("AttributeDefinitions")
            .Produces<CreateAttributeDefinitionResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
