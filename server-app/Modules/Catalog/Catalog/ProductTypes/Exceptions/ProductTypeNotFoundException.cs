using Shared.Exceptions;

namespace Catalog.ProductTypes.Exceptions
{
    public class ProductTypeNotFoundException : NotFoundException
    {
        public ProductTypeNotFoundException(Guid id) : base("ProductType", id) { }
    }
}
