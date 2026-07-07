namespace Catalog.Products.Models
{
    public class Product : AuditableSoftDeleteEntity<Guid>
    {
        public string Sku { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public string Slug { get; private set; } = default!;
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public ProductStatus Status { get; private set; }

        public Guid CategoryId { get; private set; }
        public Category Category { get; private set; } = default!;

        public Guid BrandId { get; private set; }
        public Brand Brand { get; private set; } = default!;

        public Guid ProductTypeId { get; private set; }
        public ProductType ProductType { get; private set; } = default!;

        private readonly List<ProductAttribute> _attributes = new();
        public IReadOnlyCollection<ProductAttribute> Attributes => _attributes.AsReadOnly();

        private readonly List<ProductImage> _images = new();
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

        private Product() { }

        public static Product Create(
            Guid id,
            string sku,
            string name,
            string slug,
            string? description,
            decimal price,
            Guid categoryId,
            Guid brandId,
            Guid productTypeId,
            ProductStatus status = ProductStatus.Draft)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sku);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(slug);
            ArgumentOutOfRangeException.ThrowIfNegative(price);
            if (categoryId == Guid.Empty) throw new ArgumentException("CategoryId is required.", nameof(categoryId));
            if (brandId == Guid.Empty) throw new ArgumentException("BrandId is required.", nameof(brandId));
            if (productTypeId == Guid.Empty) throw new ArgumentException("ProductTypeId is required.", nameof(productTypeId));

            return new Product
            {
                Id = id,
                Sku = sku.Trim(),
                Name = name.Trim(),
                Slug = slug.Trim().ToLowerInvariant(),
                Description = description,
                Price = price,
                CategoryId = categoryId,
                BrandId = brandId,
                ProductTypeId = productTypeId,
                Status = status
            };
        }

        public void Update(
            string name,
            string slug,
            string? description,
            decimal price,
            Guid categoryId,
            Guid brandId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(slug);
            ArgumentOutOfRangeException.ThrowIfNegative(price);
            if (categoryId == Guid.Empty) throw new ArgumentException("CategoryId is required.", nameof(categoryId));
            if (brandId == Guid.Empty) throw new ArgumentException("BrandId is required.", nameof(brandId));

            Name = name.Trim();
            Slug = slug.Trim().ToLowerInvariant();
            Description = description;
            Price = price;
            CategoryId = categoryId;
            BrandId = brandId;
        }

        public void ChangeStatus(ProductStatus status) => Status = status;

        public void SoftDelete(string by)
        {
            DeletedAt = DateTime.UtcNow;
            DeletedBy = by;
        }

        public void SetAttribute(Guid attributeDefinitionId, string value)
        {
            if (attributeDefinitionId == Guid.Empty)
                throw new ArgumentException("AttributeDefinitionId is required.", nameof(attributeDefinitionId));

            var existing = _attributes.FirstOrDefault(a => a.AttributeDefinitionId == attributeDefinitionId);
            if (existing is null)
                _attributes.Add(ProductAttribute.Create(Guid.NewGuid(), Id, attributeDefinitionId, value));
            else
                existing.SetValue(value);
        }

        public void RemoveAttribute(Guid attributeDefinitionId)
        {
            var existing = _attributes.FirstOrDefault(a => a.AttributeDefinitionId == attributeDefinitionId);
            if (existing is not null) _attributes.Remove(existing);
        }

        public void AddImage(Guid imageId, string url, string? alt, int displayOrder, bool isPrimary)
        {
            if (isPrimary)
                foreach (var img in _images) img.SetPrimary(false);
            _images.Add(ProductImage.Create(imageId, Id, url, alt, displayOrder, isPrimary));
        }

        public void RemoveImage(Guid imageId)
        {
            var img = _images.FirstOrDefault(x => x.Id == imageId);
            if (img is not null) _images.Remove(img);
        }
    }
}
