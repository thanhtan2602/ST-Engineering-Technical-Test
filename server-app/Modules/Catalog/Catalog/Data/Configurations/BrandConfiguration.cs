using Catalog.Brands.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Data.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brands");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Slug).HasMaxLength(180).IsRequired();

            builder.HasIndex(x => x.Slug)
                .IsUnique()
                .HasFilter("\"DeletedAt\" IS NULL");

            builder.HasIndex(x => x.Name);

            builder.HasQueryFilter(x => x.DeletedAt == null);
        }
    }
}
