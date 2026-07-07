namespace Catalog.Categories.Features.UpdateCategory
{
    public record UpdateCategoryRequest(string Name, string Slug, Guid? ParentId);
    public record UpdateCategoryResponse(bool IsSuccess);

    public class UpdateCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/categories/{id:guid}", async (Guid id, UpdateCategoryRequest request, ISender sender) =>
            {
                var command = new UpdateCategoryCommand(id, request.Name, request.Slug, request.ParentId);
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateCategoryResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateCategory")
            .WithTags("Categories")
            .Produces<UpdateCategoryResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
