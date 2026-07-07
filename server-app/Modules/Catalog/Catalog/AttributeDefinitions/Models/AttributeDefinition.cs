using Shared.DDD;

namespace Catalog.AttributeDefinitions.Models
{
    public class AttributeDefinition : Entity<Guid>
    {
        public string Key { get; private set; } = default!;
        public string Label { get; private set; } = default!;
        public AttributeDataType DataType { get; private set; }
        public string? Unit { get; private set; }
        public List<string> AllowedValues { get; private set; } = new();

        private AttributeDefinition() { }

        public static AttributeDefinition Create(
            Guid id,
            string key,
            string label,
            AttributeDataType dataType,
            string? unit = null,
            IEnumerable<string>? allowedValues = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentException.ThrowIfNullOrWhiteSpace(label);

            var values = (allowedValues ?? []).ToList();
            if (dataType == AttributeDataType.Enum && values.Count == 0)
                throw new InvalidOperationException("Enum-type attribute must declare AllowedValues.");

            return new AttributeDefinition
            {
                Id = id,
                Key = key.Trim().ToLowerInvariant(),
                Label = label.Trim(),
                DataType = dataType,
                Unit = string.IsNullOrWhiteSpace(unit) ? null : unit.Trim(),
                AllowedValues = values
            };
        }

        public void Update(string label, string? unit, IEnumerable<string>? allowedValues)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label);

            var values = (allowedValues ?? []).ToList();
            if (DataType == AttributeDataType.Enum && values.Count == 0)
                throw new InvalidOperationException("Enum-type attribute must declare AllowedValues.");

            Label = label.Trim();
            Unit = string.IsNullOrWhiteSpace(unit) ? null : unit.Trim();
            AllowedValues = values;
        }
    }
}
