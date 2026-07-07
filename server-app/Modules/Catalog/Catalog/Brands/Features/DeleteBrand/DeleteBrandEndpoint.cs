namespace Catalog.Brands.Features.DeleteBrand
{
    public record DeleteBrandResponse(bool IsSuccess);

    public class DeleteBrandEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/brands/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteBrandCommand(id));
                var response = result.Adapt<DeleteBrandResponse>();
                return Results.Ok(response);
            })
            .WithName("DeleteBrand")
            .WithTags("Brands")
            .Produces<DeleteBrandResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
