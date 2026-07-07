using System.Text.Json;
using Catalog.AttributeDefinitions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Data.Configurations
{
    public class AttributeDefinitionConfiguration : IEntityTypeConfiguration<AttributeDefinition>
    {
        public void Configure(EntityTypeBuilder<AttributeDefinition> builder)
        {
            builder.ToTable("AttributeDefinitions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Key).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Label).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Unit).HasMaxLength(32);

            builder.Property(x => x.DataType)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            var allowedValuesComparer = new ValueComparer<List<string>>(
                (a, b) => (a ?? new List<string>()).SequenceEqual(b ?? new List<string>()),
                v => v.Aggregate(0, (h, s) => HashCode.Combine(h, s.GetHashCode())),
                v => v.ToList());

            builder.Property(x => x.AllowedValues)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
                .Metadata.SetValueComparer(allowedValuesComparer);

            builder.HasIndex(x => x.Key).IsUnique();
        }
    }
}
