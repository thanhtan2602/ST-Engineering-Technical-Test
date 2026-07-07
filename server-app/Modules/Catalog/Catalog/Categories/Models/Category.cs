using Shared.DDD;

namespace Catalog.Categories.Models
{
    public class Category : AuditableSoftDeleteEntity<Guid>
    {
        public string Name { get; private set; } = default!;
        public string Slug { get; private set; } = default!;

        public Guid? ParentId { get; private set; }
        public Category? Parent { get; private set; }

        private readonly List<Category> _children = new();
        public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

        private Category() { }

        public static Category Create(Guid id, string name, string slug, Guid? parentId = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(slug);

            return new Category
            {
                Id = id,
                Name = name.Trim(),
                Slug = slug.Trim().ToLowerInvariant(),
                ParentId = parentId
            };
        }

        public void Update(string name, string slug, Guid? parentId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(slug);
            if (parentId == Id)
                throw new InvalidOperationException("Category cannot be its own parent.");

            Name = name.Trim();
            Slug = slug.Trim().ToLowerInvariant();
            ParentId = parentId;
        }

        public void SoftDelete(string by)
        {
            DeletedAt = DateTime.UtcNow;
            DeletedBy = by;
        }
    }
}
