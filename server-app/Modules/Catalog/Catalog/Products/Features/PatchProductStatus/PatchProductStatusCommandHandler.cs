using Catalog.Data.Repositories;

namespace Catalog.Products.Features.PatchProductStatus
{
    public record PatchProductStatusCommand(Guid Id, ProductStatus Status) : ICommand<PatchProductStatusResult>;
    public record PatchProductStatusResult(bool IsSuccess);

    public class PatchProductStatusCommandValidator : AbstractValidator<PatchProductStatusCommand>
    {
        public PatchProductStatusCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Status).IsInEnum();
        }
    }

    public class PatchProductStatusCommandHandler(IProductRepository productRepository)
        : ICommandHandler<PatchProductStatusCommand, PatchProductStatusResult>
    {
        public async Task<PatchProductStatusResult> Handle(PatchProductStatusCommand command, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetProductForUpdateAsync(command.Id, cancellationToken)
                ?? throw new ProductNotFoundException(command.Id);

            product.ChangeStatus(command.Status);
            await productRepository.SaveChangesAsync(product.Id, cancellationToken);

            return new PatchProductStatusResult(true);
        }
    }
}
