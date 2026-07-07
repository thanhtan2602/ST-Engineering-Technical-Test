using Catalog.AttributeDefinitions.Models;
using Shared.DDD;

namespace Catalog.Products.Models
{
    public class ProductAttribute : Entity<Guid>
    {
        public Guid ProductId { get; private set; }

        public Guid AttributeDefinitionId { get; private set; }
        public AttributeDefinition AttributeDefinition { get; private set; } = default!;

        public string Value { get; private set; } = default!;

        private ProductAttribute() { }

        internal static ProductAttribute Create(Guid id, Guid productId, Guid attributeDefinitionId, string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            return new ProductAttribute
            {
                Id = id,
                ProductId = productId,
                AttributeDefinitionId = attributeDefinitionId,
                Value = value.Trim()
            };
        }

        internal void SetValue(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            Value = value.Trim();
        }
    }
}
