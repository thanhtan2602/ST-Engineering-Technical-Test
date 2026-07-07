using Catalog.AttributeDefinitions.Models;

namespace Catalog.ProductTypes.Dtos
{
    public record ProductTypeAttributeDto(
        Guid AttributeDefinitionId,
        string Key,
        string Label,
        AttributeDataType DataType,
        string? Unit,
        bool IsRequired,
        int DisplayOrder,
        List<string> AllowedValues);

    public record ProductTypeDto(
        Guid Id,
        string Code,
        string Name,
        List<ProductTypeAttributeDto> Attributes);

    public record ProductTypeSummaryDto(Guid Id, string Code, string Name);
}
