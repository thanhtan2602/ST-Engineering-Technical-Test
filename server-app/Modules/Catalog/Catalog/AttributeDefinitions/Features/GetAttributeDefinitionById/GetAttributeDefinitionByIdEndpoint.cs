using Catalog.AttributeDefinitions.Dtos;

namespace Catalog.AttributeDefinitions.Features.GetAttributeDefinitionById
{
    public record GetAttributeDefinitionByIdResponse(AttributeDefinitionDto AttributeDefinition);

    public class GetAttributeDefinitionByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/attribute-definitions/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetAttributeDefinitionByIdQuery(id));
                var response = result.Adapt<GetAttributeDefinitionByIdResponse>();
                return Results.Ok(response);
            })
            .WithName("GetAttributeDefinitionById")
            .WithTags("AttributeDefinitions")
            .Produces<GetAttributeDefinitionByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
