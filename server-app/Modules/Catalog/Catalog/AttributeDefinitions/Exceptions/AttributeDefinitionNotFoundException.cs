using Shared.Exceptions;

namespace Catalog.AttributeDefinitions.Exceptions
{
    public class AttributeDefinitionNotFoundException : NotFoundException
    {
        public AttributeDefinitionNotFoundException(Guid id) : base("AttributeDefinition", id) { }
    }
}
