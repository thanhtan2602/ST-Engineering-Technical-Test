using Catalog.AttributeDefinitions.Models;

namespace Catalog.AttributeDefinitions.Dtos
{
    public record AttributeDefinitionDto(
        Guid Id,
        string Key,
        string Label,
        AttributeDataType DataType,
        string? Unit,
        List<string> AllowedValues);
}
