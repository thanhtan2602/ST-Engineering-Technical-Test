namespace Catalog.Categories.Dtos
{
    public record CategoryDto(Guid Id, string Name, string Slug, Guid? ParentId);
}
