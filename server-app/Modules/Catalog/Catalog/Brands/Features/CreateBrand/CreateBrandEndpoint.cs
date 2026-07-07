namespace Catalog.Brands.Features.CreateBrand
{
    public record CreateBrandRequest(string Name, string Slug);
    public record CreateBrandResponse(Guid Id);

    public class CreateBrandEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/brands", async (CreateBrandRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateBrandCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateBrandResponse>();
                return Results.Created($"/brands/{response.Id}", response);
            })
            .WithName("CreateBrand")
            .WithTags("Brands")
            .Produces<CreateBrandResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
