using Catalog.AttributeDefinitions.Dtos;

namespace Catalog.AttributeDefinitions.Features.ListAttributeDefinitions
{
    public record ListAttributeDefinitionsResponse(PaginatedResult<AttributeDefinitionDto> AttributeDefinitions);

    public class ListAttributeDefinitionsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/attribute-definitions", async (
                [AsParameters] PaginationRequest pagination,
                string? search,
                ISender sender) =>
            {
                var result = await sender.Send(new ListAttributeDefinitionsQuery(pagination, search));
                var response = result.Adapt<ListAttributeDefinitionsResponse>();
                return Results.Ok(response);
            })
            .WithName("ListAttributeDefinitions")
            .WithTags("AttributeDefinitions")
            .Produces<ListAttributeDefinitionsResponse>();
        }
    }
}
