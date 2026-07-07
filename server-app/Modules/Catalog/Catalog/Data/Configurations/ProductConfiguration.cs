using Catalog.Products.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Sku).HasMaxLength(64).IsRequired();
            builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Slug).HasMaxLength(220).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(2000);

            builder.Property(p => p.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            // Postgres xmin optimistic concurrency
            builder.UseXminAsConcurrencyToken();

            // Relationships (aggregate owns Attributes and Images)
            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ProductType)
                .WithMany()
                .HasForeignKey(p => p.ProductTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Attributes)
                .WithOne()
                .HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Images)
                .WithOne()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Backing-field access for read-only collections
            builder.Metadata.FindNavigation(nameof(Product.Attributes))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.Metadata.FindNavigation(nameof(Product.Images))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // Indexes
            builder.HasIndex(p => p.Sku)
                .IsUnique()
                .HasFilter("\"DeletedAt\" IS NULL");

            builder.HasIndex(p => p.Slug)
                .IsUnique()
                .HasFilter("\"DeletedAt\" IS NULL");

            builder.HasIndex(p => p.Name);
            builder.HasIndex(p => new { p.Status, p.CategoryId });
            builder.HasIndex(p => p.BrandId);
            builder.HasIndex(p => p.ProductTypeId);

            // Soft-delete filter
            builder.HasQueryFilter(p => p.DeletedAt == null);
        }
    }
}
