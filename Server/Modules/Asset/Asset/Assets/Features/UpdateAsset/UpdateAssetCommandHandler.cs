using Asset.Data.Repository;
using Shared.CQRS;

namespace Asset.Assets.Features.UpdateAsset;

public sealed class UpdateAssetCommandHandler(IAssetRepository assetRepository)
    : ICommandHandler<UpdateAssetCommand, UpdateAssetResult>
{
    private readonly IAssetRepository _repository = assetRepository;

    public async Task<UpdateAssetResult> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (asset is null)
            return new UpdateAssetResult(false, "Asset not found.");

        asset.Update(
            request.Brand,
            request.Name,
            request.Description,
            request.Type,
            request.Status,
            request.Amount,
            request.Remark);

        await _repository.UpdateAsync(asset, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new UpdateAssetResult(true, null);
    }
}
