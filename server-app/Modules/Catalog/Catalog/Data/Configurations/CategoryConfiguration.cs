using Catalog.Categories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Slug).HasMaxLength(180).IsRequired();

            // Self-reference for hierarchy
            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Metadata.FindNavigation(nameof(Category.Children))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(x => x.Slug)
                .IsUnique()
                .HasFilter("\"DeletedAt\" IS NULL");

            builder.HasIndex(x => x.ParentId);

            builder.HasQueryFilter(x => x.DeletedAt == null);
        }
    }
}
