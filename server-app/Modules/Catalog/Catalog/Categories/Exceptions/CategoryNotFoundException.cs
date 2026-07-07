using Shared.Exceptions;

namespace Catalog.Categories.Exceptions
{
    public class CategoryNotFoundException : NotFoundException
    {
        public CategoryNotFoundException(Guid id) : base("Category", id) { }
    }
}
