namespace Catalog.Products.Dtos
{
    public record ProductAttributeInput(Guid AttributeDefinitionId, string Value);

    public record ProductImageInput(string Url, string? Alt, int DisplayOrder, bool IsPrimary);

    public record ProductAttributeValueDto(
        Guid AttributeDefinitionId,
        string Key,
        string Label,
        string Value,
        string DataType);

    public record ProductImageDto(
        Guid Id,
        string Url,
        string? Alt,
        int DisplayOrder,
        bool IsPrimary);

    public record ProductDto(
        Guid Id,
        string Sku,
        string Name,
        string Slug,
        string? Description,
        decimal Price,
        string Status,
        Guid CategoryId,
        string CategoryName,
        Guid BrandId,
        string BrandName,
        Guid ProductTypeId,
        string ProductTypeName,
        uint RowVersion,
        DateTime? CreatedAt,
        DateTime? LastModifiedAt,
        List<ProductAttributeValueDto> Attributes,
        List<ProductImageDto> Images);

    public record ProductSummaryDto(
        Guid Id,
        string Sku,
        string Name,
        string Slug,
        decimal Price,
        string Status,
        Guid CategoryId,
        string CategoryName,
        Guid BrandId,
        string BrandName,
        Guid ProductTypeId,
        string ProductTypeName,
        string? PrimaryImageUrl);
}
