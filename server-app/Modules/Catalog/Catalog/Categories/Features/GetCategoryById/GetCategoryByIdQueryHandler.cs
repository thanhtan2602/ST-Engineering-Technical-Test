using Catalog.Categories.Dtos;
using Catalog.Categories.Exceptions;

namespace Catalog.Categories.Features.GetCategoryById
{
    public record GetCategoryByIdQuery(Guid Id) : IQuery<GetCategoryByIdResult>;

    public record GetCategoryByIdResult(CategoryDto Category);

    public class GetCategoryByIdQueryHandler(CatalogDbContext dbContext)
        : IQueryHandler<GetCategoryByIdQuery, GetCategoryByIdResult>
    {
        public async Task<GetCategoryByIdResult> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            var c = await dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken)
                ?? throw new CategoryNotFoundException(query.Id);

            return new GetCategoryByIdResult(new CategoryDto(c.Id, c.Name, c.Slug, c.ParentId));
        }
    }
}
