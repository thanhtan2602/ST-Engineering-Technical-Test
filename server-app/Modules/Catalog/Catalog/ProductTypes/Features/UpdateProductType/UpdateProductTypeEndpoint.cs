namespace Catalog.ProductTypes.Features.UpdateProductType
{
    public record UpdateProductTypeRequest(string Name);
    public record UpdateProductTypeResponse(bool IsSuccess);

    public class UpdateProductTypeEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/product-types/{id:guid}", async (Guid id, UpdateProductTypeRequest request, ISender sender) =>
            {
                var command = new UpdateProductTypeCommand(id, request.Name);
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateProductTypeResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateProductType")
            .WithTags("ProductTypes")
            .Produces<UpdateProductTypeResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
