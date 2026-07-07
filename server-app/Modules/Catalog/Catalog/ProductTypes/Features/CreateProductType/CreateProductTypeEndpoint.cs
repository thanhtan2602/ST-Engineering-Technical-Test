namespace Catalog.ProductTypes.Features.CreateProductType
{
    public record CreateProductTypeRequest(string Code, string Name);
    public record CreateProductTypeResponse(Guid Id);

    public class CreateProductTypeEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/product-types", async (CreateProductTypeRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateProductTypeCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateProductTypeResponse>();
                return Results.Created($"/product-types/{response.Id}", response);
            })
            .WithName("CreateProductType")
            .WithTags("ProductTypes")
            .Produces<CreateProductTypeResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
