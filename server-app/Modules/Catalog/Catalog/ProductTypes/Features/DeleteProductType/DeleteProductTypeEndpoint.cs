namespace Catalog.ProductTypes.Features.DeleteProductType
{
    public record DeleteProductTypeResponse(bool IsSuccess);

    public class DeleteProductTypeEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/product-types/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteProductTypeCommand(id));
                var response = result.Adapt<DeleteProductTypeResponse>();
                return Results.Ok(response);
            })
            .WithName("DeleteProductType")
            .WithTags("ProductTypes")
            .Produces<DeleteProductTypeResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
