using System.Globalization;
using Catalog.AttributeDefinitions.Models;
using Catalog.Products.Dtos;
using Shared.Exceptions;

namespace Catalog.Products.Services
{
    // Validates ProductAttribute inputs against the constraints defined by ProductType.
    // Expects productType.TypeAttributes and their AttributeDefinition to be eagerly loaded.
    internal static class ProductAttributeValidator
    {
        public static void Validate(ProductType productType, IReadOnlyCollection<ProductAttributeInput> inputs)
        {
            var errors = new List<string>();
            var allowedByDefId = productType.TypeAttributes
                .ToDictionary(ta => ta.AttributeDefinitionId);

            // Required attributes must be present with a non-empty value
            foreach (var required in productType.TypeAttributes.Where(t => t.IsRequired))
            {
                var input = inputs.FirstOrDefault(i => i.AttributeDefinitionId == required.AttributeDefinitionId);
                if (input is null || string.IsNullOrWhiteSpace(input.Value))
                    errors.Add($"Attribute '{required.AttributeDefinition.Key}' is required.");
            }

            // Every input must belong to the ProductType's attribute set, and value must fit its DataType
            foreach (var input in inputs)
            {
                if (!allowedByDefId.TryGetValue(input.AttributeDefinitionId, out var typeAttr))
                {
                    errors.Add($"Attribute {input.AttributeDefinitionId} is not defined for ProductType '{productType.Code}'.");
                    continue;
                }

                var valueError = ValidateValue(typeAttr.AttributeDefinition, input.Value);
                if (valueError is not null)
                    errors.Add(valueError);
            }

            if (errors.Count > 0)
                throw new BadRequestException("Product attribute validation failed.", string.Join("; ", errors));
        }

        // Validates a single attribute upsert: whitelist + DataType only (no required-presence check).
        public static void ValidateSingle(ProductType productType, ProductAttributeInput input)
        {
            var typeAttr = productType.TypeAttributes
                .FirstOrDefault(ta => ta.AttributeDefinitionId == input.AttributeDefinitionId)
                ?? throw new BadRequestException(
                    $"Attribute {input.AttributeDefinitionId} is not defined for ProductType '{productType.Code}'.");

            var error = ValidateValue(typeAttr.AttributeDefinition, input.Value);
            if (error is not null)
                throw new BadRequestException("Product attribute validation failed.", error);
        }

        private static string? ValidateValue(AttributeDefinition def, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return $"Attribute '{def.Key}' value is empty.";

            switch (def.DataType)
            {
                case AttributeDataType.Text:
                    return null;

                case AttributeDataType.Number:
                    return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _)
                        ? null
                        : $"Attribute '{def.Key}' must be a number.";

                case AttributeDataType.Boolean:
                    return bool.TryParse(value, out _)
                        ? null
                        : $"Attribute '{def.Key}' must be true or false.";

                case AttributeDataType.Date:
                    return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _)
                        ? null
                        : $"Attribute '{def.Key}' must be a valid ISO 8601 date.";

                case AttributeDataType.Enum:
                    return def.AllowedValues.Contains(value)
                        ? null
                        : $"Attribute '{def.Key}' value '{value}' is not in allowed values: [{string.Join(", ", def.AllowedValues)}].";

                default:
                    return null;
            }
        }
    }
}
