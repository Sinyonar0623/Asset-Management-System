using Asset.Assets.Features.AssetFeature.CreateAsset;

namespace Asset.Service.CommandHandlerService;

public interface IAssetCommandHandlerService
{
    Task<Guid> CreateAsset(AssetDto asset, List<Guid> units, CancellationToken cancellationToken);
    Task AssignAssetUnits(Guid assetId, Assets.Model.Asset asset, List<Guid> units, CancellationToken cancellationToken);
}