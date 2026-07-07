using Catalog.AttributeDefinitions.Models;

namespace Catalog.ProductTypes.Models
{
    public class ProductTypeAttribute
    {
        public Guid ProductTypeId { get; private set; }
        public ProductType ProductType { get; private set; } = default!;

        public Guid AttributeDefinitionId { get; private set; }
        public AttributeDefinition AttributeDefinition { get; private set; } = default!;

        public bool IsRequired { get; private set; }
        public int DisplayOrder { get; private set; }

        private ProductTypeAttribute() { }

        internal static ProductTypeAttribute Create(
            Guid productTypeId,
            Guid attributeDefinitionId,
            bool isRequired,
            int displayOrder)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(displayOrder);

            return new ProductTypeAttribute
            {
                ProductTypeId = productTypeId,
                AttributeDefinitionId = attributeDefinitionId,
                IsRequired = isRequired,
                DisplayOrder = displayOrder
            };
        }

        internal void SetRequired(bool value) => IsRequired = value;

        internal void SetDisplayOrder(int order)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(order);
            DisplayOrder = order;
        }
    }
}
