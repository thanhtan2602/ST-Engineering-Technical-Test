using Shared.DDD;

namespace Catalog.Products.Models
{
    public class ProductImage : Entity<Guid>
    {
        public Guid ProductId { get; private set; }
        public string Url { get; private set; } = default!;
        public string? Alt { get; private set; }
        public int DisplayOrder { get; private set; }
        public bool IsPrimary { get; private set; }

        private ProductImage() { }

        internal static ProductImage Create(Guid id, Guid productId, string url, string? alt, int displayOrder, bool isPrimary)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(url);
            ArgumentOutOfRangeException.ThrowIfNegative(displayOrder);

            return new ProductImage
            {
                Id = id,
                ProductId = productId,
                Url = url.Trim(),
                Alt = alt,
                DisplayOrder = displayOrder,
                IsPrimary = isPrimary
            };
        }

        internal void SetPrimary(bool value) => IsPrimary = value;

        internal void UpdateMeta(string? alt, int displayOrder)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(displayOrder);
            Alt = alt;
            DisplayOrder = displayOrder;
        }
    }
}
