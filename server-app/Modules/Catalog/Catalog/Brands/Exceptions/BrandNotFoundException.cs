using Shared.Exceptions;

namespace Catalog.Brands.Exceptions
{
    public class BrandNotFoundException : NotFoundException
    {
        public BrandNotFoundException(Guid id) : base("Brand", id) { }
    }
}
