using Catalog.ProductTypes.Dtos;

namespace Catalog.ProductTypes.Features.GetProductTypeById
{
    public record GetProductTypeByIdResponse(ProductTypeDto ProductType);

    public class GetProductTypeByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/product-types/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetProductTypeByIdQuery(id));
                var response = result.Adapt<GetProductTypeByIdResponse>();
                return Results.Ok(response);
            })
            .WithName("GetProductTypeById")
            .WithTags("ProductTypes")
            .Produces<GetProductTypeByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
