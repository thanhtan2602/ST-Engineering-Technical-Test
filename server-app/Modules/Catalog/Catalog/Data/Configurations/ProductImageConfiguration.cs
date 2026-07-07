using Catalog.Products.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Data.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImages");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Alt).HasMaxLength(200);

            builder.HasIndex(x => new { x.ProductId, x.DisplayOrder });
        }
    }
}
