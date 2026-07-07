using Catalog.Products.Dtos;

namespace Catalog.Products.Features.UploadProductImage
{
    public record UploadProductImageResponse(ProductImageDto Image);

    public class UploadProductImageEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products/{id:guid}/images", async (
                Guid id,
                IFormFile file,
                IFormCollection form,
                IWebHostEnvironment env,
                ISender sender) =>
            {
                var alt = form["alt"].FirstOrDefault();
                int.TryParse(form["displayOrder"].FirstOrDefault(), out var displayOrder);
                bool.TryParse(form["isPrimary"].FirstOrDefault(), out var isPrimary);

                var command = new UploadProductImageCommand(id, file, alt, displayOrder, isPrimary, env.WebRootPath);
                var result = await sender.Send(command);
                return Results.Created($"/api/v1/products/{id}", result.Adapt<UploadProductImageResponse>());
            })
            .WithName("UploadProductImage")
            .WithTags("Products")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<UploadProductImageResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .DisableAntiforgery();
        }
    }
}
