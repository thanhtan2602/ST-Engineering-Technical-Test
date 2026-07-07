using Shared.DDD;

namespace Catalog.ProductTypes.Models
{
    public class ProductType : AuditableSoftDeleteEntity<Guid>
    {
        public string Code { get; private set; } = default!;
        public string Name { get; private set; } = default!;

        private readonly List<ProductTypeAttribute> _typeAttributes = new();
        public IReadOnlyCollection<ProductTypeAttribute> TypeAttributes => _typeAttributes.AsReadOnly();

        private ProductType() { }

        public static ProductType Create(Guid id, string code, string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            return new ProductType
            {
                Id = id,
                Code = code.Trim().ToLowerInvariant(),
                Name = name.Trim()
            };
        }

        public void Update(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            Name = name.Trim();
        }

        public void AttachAttribute(Guid attributeDefinitionId, bool isRequired, int displayOrder)
        {
            var existing = _typeAttributes.FirstOrDefault(a => a.AttributeDefinitionId == attributeDefinitionId);
            if (existing is not null)
            {
                existing.SetRequired(isRequired);
                existing.SetDisplayOrder(displayOrder);
                return;
            }
            _typeAttributes.Add(ProductTypeAttribute.Create(Id, attributeDefinitionId, isRequired, displayOrder));
        }

        public void DetachAttribute(Guid attributeDefinitionId)
        {
            var existing = _typeAttributes.FirstOrDefault(a => a.AttributeDefinitionId == attributeDefinitionId);
            if (existing is not null) _typeAttributes.Remove(existing);
        }

        public void SoftDelete(string by)
        {
            DeletedAt = DateTime.UtcNow;
            DeletedBy = by;
        }
    }
}
