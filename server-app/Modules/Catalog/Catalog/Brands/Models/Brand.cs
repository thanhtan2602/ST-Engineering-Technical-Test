using Shared.DDD;

namespace Catalog.Brands.Models
{
    public class Brand : AuditableSoftDeleteEntity<Guid>
    {
        public string Name { get; private set; } = default!;
        public string Slug { get; private set; } = default!;

        private Brand() { }

        public static Brand Create(Guid id, string name, string slug)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(slug);

            return new Brand
            {
                Id = id,
                Name = name.Trim(),
                Slug = slug.Trim().ToLowerInvariant()
            };
        }

        public void Update(string name, string slug)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(slug);

            Name = name.Trim();
            Slug = slug.Trim().ToLowerInvariant();
        }

        public void SoftDelete(string by)
        {
            DeletedAt = DateTime.UtcNow;
            DeletedBy = by;
        }
    }
}
