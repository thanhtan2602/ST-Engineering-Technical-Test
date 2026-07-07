namespace Catalog.Products.Features.PatchProductStatus
{
    public record PatchProductStatusRequest(ProductStatus Status);
    public record PatchProductStatusResponse(bool IsSuccess);

    public class PatchProductStatusEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/products/{id:guid}/status", async (Guid id, PatchProductStatusRequest request, ISender sender) =>
            {
                var result = await sender.Send(new PatchProductStatusCommand(id, request.Status));
                return Results.Ok(result.Adapt<PatchProductStatusResponse>());
            })
            .WithName("PatchProductStatus")
            .WithTags("Products")
            .Produces<PatchProductStatusResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
