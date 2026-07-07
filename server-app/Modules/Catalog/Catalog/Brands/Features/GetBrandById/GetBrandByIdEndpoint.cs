using Catalog.Brands.Dtos;

namespace Catalog.Brands.Features.GetBrandById
{
    public record GetBrandByIdResponse(BrandDto Brand);

    public class GetBrandByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/brands/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetBrandByIdQuery(id));
                var response = result.Adapt<GetBrandByIdResponse>();
                return Results.Ok(response);
            })
            .WithName("GetBrandById")
            .WithTags("Brands")
            .Produces<GetBrandByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
