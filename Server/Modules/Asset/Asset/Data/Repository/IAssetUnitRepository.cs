using Asset.Assets.Model;
using Shared.Data.Repository;

namespace Asset.Data.Repository;

public interface IAssetUnitRepository : IRepository<AssetUnit, long>
{
    Task<AssetUnit?> GetByIdWithHistoriesAsync(long assetUnitId, CancellationToken cancellationToken = default);
    Task<bool> AssetTagExistsAsync(string assetTag, long? excludeAssetUnitId = null, CancellationToken cancellationToken = default);
    Task<bool> SerialNoExistsAsync(string serialNo, long? excludeAssetUnitId = null, CancellationToken cancellationToken = default);
}
