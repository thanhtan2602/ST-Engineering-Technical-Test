namespace Catalog.Categories.Features.DeleteCategory
{
    public record DeleteCategoryResponse(bool IsSuccess);

    public class DeleteCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/categories/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteCategoryCommand(id));
                var response = result.Adapt<DeleteCategoryResponse>();
                return Results.Ok(response);
            })
            .WithName("DeleteCategory")
            .WithTags("Categories")
            .Produces<DeleteCategoryResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
