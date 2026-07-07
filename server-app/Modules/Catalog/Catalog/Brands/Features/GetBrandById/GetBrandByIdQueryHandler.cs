using Catalog.Brands.Dtos;
using Catalog.Brands.Exceptions;

namespace Catalog.Brands.Features.GetBrandById
{
    public record GetBrandByIdQuery(Guid Id) : IQuery<GetBrandByIdResult>;

    public record GetBrandByIdResult(BrandDto Brand);

    public class GetBrandByIdQueryHandler(CatalogDbContext dbContext)
        : IQueryHandler<GetBrandByIdQuery, GetBrandByIdResult>
    {
        public async Task<GetBrandByIdResult> Handle(GetBrandByIdQuery query, CancellationToken cancellationToken)
        {
            var brand = await dbContext.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == query.Id, cancellationToken)
                ?? throw new BrandNotFoundException(query.Id);

            return new GetBrandByIdResult(new BrandDto(brand.Id, brand.Name, brand.Slug));
        }
    }
}
