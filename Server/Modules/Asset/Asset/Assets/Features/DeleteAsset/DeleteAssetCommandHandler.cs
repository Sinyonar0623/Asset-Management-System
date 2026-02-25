using Asset.Data.Repository;
using Shared.CQRS;

namespace Asset.Assets.Features.DeleteAsset;

public sealed class DeleteAssetCommandHandler(IAssetRepository assetRepository)
    : ICommandHandler<DeleteAssetCommand, DeleteAssetResult>
{
    private readonly IAssetRepository _repository = assetRepository;

    public async Task<DeleteAssetResult> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (asset is null)
            return new DeleteAssetResult(false, "Asset not found.");

        await _repository.DeleteAsync(asset, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new DeleteAssetResult(true, null);
    }
}
