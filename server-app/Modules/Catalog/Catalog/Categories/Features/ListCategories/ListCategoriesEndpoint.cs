using Catalog.Categories.Dtos;

namespace Catalog.Categories.Features.ListCategories
{
    public record ListCategoriesResponse(PaginatedResult<CategoryDto> Categories);

    public class ListCategoriesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/categories", async (
                [AsParameters] PaginationRequest pagination,
                string? search,
                Guid? parentId,
                ISender sender) =>
            {
                var result = await sender.Send(new ListCategoriesQuery(pagination, search, parentId));
                var response = result.Adapt<ListCategoriesResponse>();
                return Results.Ok(response);
            })
            .WithName("ListCategories")
            .WithTags("Categories")
            .Produces<ListCategoriesResponse>();
        }
    }
}
