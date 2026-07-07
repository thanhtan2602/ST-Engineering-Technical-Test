using Catalog.Data.Repositories;
using Catalog.Products.Dtos;

namespace Catalog.Products.Features.GetProductById
{
    public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;

    public record GetProductByIdResult(ProductDto Product);

    public class GetProductByIdQueryHandler(IProductRepository productRepository)
        : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var dto = await productRepository.GetProductByIdAsync(query.Id, cancellationToken)
                ?? throw new ProductNotFoundException(query.Id);

            return new GetProductByIdResult(dto);
        }
    }
}
