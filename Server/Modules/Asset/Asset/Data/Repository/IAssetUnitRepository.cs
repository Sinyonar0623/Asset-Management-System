using Asset.Assets.Model;
using Shared.Data.Repository;

namespace Asset.Data.Repository;

public interface IAssetUnitRepository : IRepository<AssetUnit, Guid>
{
    Task<AssetUnit?> GetByIdWithHistoriesAsync(Guid assetUnitId, CancellationToken cancellationToken = default);
    Task<bool> AssetTagExistsAsync(string assetTag, Guid? excludeAssetUnitId = null, CancellationToken cancellationToken = default);
    Task<bool> SerialNoExistsAsync(string serialNo, Guid? excludeAssetUnitId = null, CancellationToken cancellationToken = default);
}
