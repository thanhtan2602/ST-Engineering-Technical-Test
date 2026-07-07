using Catalog.AttributeDefinitions.Dtos;
using Catalog.AttributeDefinitions.Exceptions;

namespace Catalog.AttributeDefinitions.Features.GetAttributeDefinitionById
{
    public record GetAttributeDefinitionByIdQuery(Guid Id) : IQuery<GetAttributeDefinitionByIdResult>;

    public record GetAttributeDefinitionByIdResult(AttributeDefinitionDto AttributeDefinition);

    public class GetAttributeDefinitionByIdQueryHandler(CatalogDbContext dbContext)
        : IQueryHandler<GetAttributeDefinitionByIdQuery, GetAttributeDefinitionByIdResult>
    {
        public async Task<GetAttributeDefinitionByIdResult> Handle(GetAttributeDefinitionByIdQuery query, CancellationToken cancellationToken)
        {
            var attr = await dbContext.AttributeDefinitions
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == query.Id, cancellationToken)
                ?? throw new AttributeDefinitionNotFoundException(query.Id);

            var dto = new AttributeDefinitionDto(attr.Id, attr.Key, attr.Label, attr.DataType, attr.Unit, attr.AllowedValues);
            return new GetAttributeDefinitionByIdResult(dto);
        }
    }
}
