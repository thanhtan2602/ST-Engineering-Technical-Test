using Catalog.Products.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Data.Configurations
{
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.ToTable("ProductAttributes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value)
                .HasMaxLength(1000)
                .IsRequired();

            builder.HasOne(x => x.AttributeDefinition)
                .WithMany()
                .HasForeignKey(x => x.AttributeDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Uniqueness: one value per (Product, AttributeDefinition)
            builder.HasIndex(x => new { x.ProductId, x.AttributeDefinitionId })
                .IsUnique();

            // Filter-by-attribute-value queries (EAV lookup)
            builder.HasIndex(x => new { x.AttributeDefinitionId, x.Value });
        }
    }
}
