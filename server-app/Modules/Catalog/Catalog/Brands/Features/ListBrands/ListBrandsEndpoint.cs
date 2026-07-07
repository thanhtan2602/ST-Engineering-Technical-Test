using Catalog.Brands.Dtos;

namespace Catalog.Brands.Features.ListBrands
{
    public record ListBrandsResponse(PaginatedResult<BrandDto> Brands);

    public class ListBrandsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/brands", async (
                [Microsoft.AspNetCore.Http.AsParameters] PaginationRequest pagination,
                string? search,
                ISender sender) =>
            {
                var result = await sender.Send(new ListBrandsQuery(pagination, search));
                var response = result.Adapt<ListBrandsResponse>();
                return Results.Ok(response);
            })
            .WithName("ListBrands")
            .WithTags("Brands")
            .Produces<ListBrandsResponse>();
        }
    }
}
