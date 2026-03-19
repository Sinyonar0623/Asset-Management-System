using Asset.Assets.Model;

namespace Asset.Service.EventHandlerService;

public interface IAssetUnitEventHandlerService
{
    Task<AssetUnit?> GetAssetUnitById(Guid assetUnitId, CancellationToken cancellationToken = default);
    Task<bool> AssignAssetUnit(Assets.Model.Asset asset, List<Guid> assetUnitId, CancellationToken cancellationToken);
}
