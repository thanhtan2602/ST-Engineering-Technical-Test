using Catalog.Categories.Dtos;

namespace Catalog.Categories.Features.GetCategoryById
{
    public record GetCategoryByIdResponse(CategoryDto Category);

    public class GetCategoryByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/categories/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetCategoryByIdQuery(id));
                var response = result.Adapt<GetCategoryByIdResponse>();
                return Results.Ok(response);
            })
            .WithName("GetCategoryById")
            .WithTags("Categories")
            .Produces<GetCategoryByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
