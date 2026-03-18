using Asset.Assets.Model;

namespace Asset.Service.EventHandlerService;

public interface IAssetUnitEventHandlerService
{
    Task<AssetUnit?> GetAssetUnitById(Guid assetUnitId, CancellationToken cancellationToken = default);
}
