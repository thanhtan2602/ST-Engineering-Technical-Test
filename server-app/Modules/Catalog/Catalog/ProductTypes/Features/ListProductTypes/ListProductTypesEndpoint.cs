using Catalog.ProductTypes.Dtos;

namespace Catalog.ProductTypes.Features.ListProductTypes
{
    public record ListProductTypesResponse(PaginatedResult<ProductTypeSummaryDto> ProductTypes);

    public class ListProductTypesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/product-types", async (
                [AsParameters] PaginationRequest pagination,
                string? search,
                ISender sender) =>
            {
                var result = await sender.Send(new ListProductTypesQuery(pagination, search));
                var response = result.Adapt<ListProductTypesResponse>();
                return Results.Ok(response);
            })
            .WithName("ListProductTypes")
            .WithTags("ProductTypes")
            .Produces<ListProductTypesResponse>();
        }
    }
}
