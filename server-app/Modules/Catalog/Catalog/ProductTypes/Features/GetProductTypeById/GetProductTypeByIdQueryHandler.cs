using Catalog.ProductTypes.Dtos;
using Catalog.ProductTypes.Exceptions;

namespace Catalog.ProductTypes.Features.GetProductTypeById
{
    public record GetProductTypeByIdQuery(Guid Id) : IQuery<GetProductTypeByIdResult>;

    public record GetProductTypeByIdResult(ProductTypeDto ProductType);

    public class GetProductTypeByIdQueryHandler(CatalogDbContext dbContext)
        : IQueryHandler<GetProductTypeByIdQuery, GetProductTypeByIdResult>
    {
        public async Task<GetProductTypeByIdResult> Handle(GetProductTypeByIdQuery query, CancellationToken cancellationToken)
        {
            var pt = await dbContext.ProductTypes
                .AsNoTracking()
                .Include(x => x.TypeAttributes)
                    .ThenInclude(ta => ta.AttributeDefinition)
                .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken)
                ?? throw new ProductTypeNotFoundException(query.Id);

            var attributes = pt.TypeAttributes
                .OrderBy(ta => ta.DisplayOrder)
                .Select(ta => new ProductTypeAttributeDto(
                    ta.AttributeDefinitionId,
                    ta.AttributeDefinition.Key,
                    ta.AttributeDefinition.Label,
                    ta.AttributeDefinition.DataType,
                    ta.AttributeDefinition.Unit,
                    ta.IsRequired,
                    ta.DisplayOrder,
                    ta.AttributeDefinition.AllowedValues))
                .ToList();

            return new GetProductTypeByIdResult(new ProductTypeDto(pt.Id, pt.Code, pt.Name, attributes));
        }
    }
}
