using Catalog.Products.Dtos;

namespace Catalog.Products.Features.ListProducts
{
    public record ListProductsResponse(PaginatedResult<ProductSummaryDto> Products);

    public class ListProductsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (
                [AsParameters] PaginationRequest pagination,
                string? search,
                Guid? categoryId,
                Guid? brandId,
                Guid? productTypeId,
                ProductStatus? status,
                decimal? minPrice,
                decimal? maxPrice,
                ProductSortOrder? sort,
                ISender sender) =>
            {
                var result = await sender.Send(
                    new ListProductsQuery(pagination, search, categoryId, brandId, productTypeId, status, minPrice, maxPrice, sort ?? ProductSortOrder.Newest));
                var response = result.Adapt<ListProductsResponse>();
                return Results.Ok(response);
            })
            .WithName("ListProducts")
            .WithTags("Products")
            .Produces<ListProductsResponse>();
        }
    }
}
