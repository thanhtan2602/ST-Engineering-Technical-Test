using Catalog.ProductTypes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Data.Configurations
{
    public class ProductTypeAttributeConfiguration : IEntityTypeConfiguration<ProductTypeAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductTypeAttribute> builder)
        {
            builder.ToTable("ProductTypeAttributes");

            // Composite PK — pure M:N join with payload
            builder.HasKey(x => new { x.ProductTypeId, x.AttributeDefinitionId });

            // Match the ProductType soft-delete filter so EF doesn't warn about
            // a required principal being filtered out.
            builder.HasQueryFilter(x => x.ProductType.DeletedAt == null);

            builder.HasOne(x => x.AttributeDefinition)
                .WithMany()
                .HasForeignKey(x => x.AttributeDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.AttributeDefinitionId);
        }
    }
}
