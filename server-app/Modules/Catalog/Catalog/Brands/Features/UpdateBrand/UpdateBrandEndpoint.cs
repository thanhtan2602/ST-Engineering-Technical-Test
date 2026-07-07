namespace Catalog.Brands.Features.UpdateBrand
{
    public record UpdateBrandRequest(string Name, string Slug);
    public record UpdateBrandResponse(bool IsSuccess);

    public class UpdateBrandEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/brands/{id:guid}", async (Guid id, UpdateBrandRequest request, ISender sender) =>
            {
                var command = new UpdateBrandCommand(id, request.Name, request.Slug);
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateBrandResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateBrand")
            .WithTags("Brands")
            .Produces<UpdateBrandResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
