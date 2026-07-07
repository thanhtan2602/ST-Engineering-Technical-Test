using Catalog.ProductTypes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Data.Configurations
{
    public class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
    {
        public void Configure(EntityTypeBuilder<ProductType> builder)
        {
            builder.ToTable("ProductTypes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();

            builder.HasMany(x => x.TypeAttributes)
                .WithOne(x => x.ProductType)
                .HasForeignKey(x => x.ProductTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(ProductType.TypeAttributes))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(x => x.Code)
                .IsUnique()
                .HasFilter("\"DeletedAt\" IS NULL");

            builder.HasQueryFilter(x => x.DeletedAt == null);
        }
    }
}
