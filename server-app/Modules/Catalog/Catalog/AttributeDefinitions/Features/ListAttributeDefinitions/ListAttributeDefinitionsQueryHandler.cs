using Catalog.AttributeDefinitions.Dtos;

namespace Catalog.AttributeDefinitions.Features.ListAttributeDefinitions
{
    public record ListAttributeDefinitionsQuery(PaginationRequest PaginationRequest, string? Search)
        : IQuery<ListAttributeDefinitionsResult>;

    public record ListAttributeDefinitionsResult(PaginatedResult<AttributeDefinitionDto> AttributeDefinitions);

    public class ListAttributeDefinitionsQueryHandler(CatalogDbContext dbContext)
        : IQueryHandler<ListAttributeDefinitionsQuery, ListAttributeDefinitionsResult>
    {
        public async Task<ListAttributeDefinitionsResult> Handle(ListAttributeDefinitionsQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var q = dbContext.AttributeDefinitions.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim().ToLower();
                q = q.Where(a => a.Key.Contains(s) || a.Label.ToLower().Contains(s));
            }

            var totalCount = await q.LongCountAsync(cancellationToken);

            var items = await q
                .OrderBy(a => a.Key)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(a => new AttributeDefinitionDto(a.Id, a.Key, a.Label, a.DataType, a.Unit, a.AllowedValues))
                .ToListAsync(cancellationToken);

            return new ListAttributeDefinitionsResult(
                new PaginatedResult<AttributeDefinitionDto>(pageIndex, pageSize, totalCount, items));
        }
    }
}
