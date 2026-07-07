namespace Catalog.Products.Features.DeleteProductImage
{
    public record DeleteProductImageResponse(bool IsSuccess);

    public class DeleteProductImageEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/products/{id:guid}/images/{imageId:guid}",
                async (Guid id, Guid imageId, ISender sender) =>
                {
                    var result = await sender.Send(new DeleteProductImageCommand(id, imageId));
                    return Results.Ok(result.Adapt<DeleteProductImageResponse>());
                })
            .WithName("DeleteProductImage")
            .WithTags("Products")
            .Produces<DeleteProductImageResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
