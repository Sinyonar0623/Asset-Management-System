using Asset.Assets.Model;
using Shared.Data;

namespace Asset.Data.Repository.Write;

public interface IAssetUnitWriteRepository : IRepository<AssetUnit, Guid>
{
    Task<AssetUnit?> GetByIdWithHistoriesAsync(Guid assetUnitId, CancellationToken cancellationToken = default);

    Task<bool> AssetTagExistsAsync(
        string assetTag,
        Guid? excludeAssetUnitId = null,
        CancellationToken cancellationToken = default);

    Task<bool> SerialNoExistsAsync(
        string serialNo,
        Guid? excludeAssetUnitId = null,
        CancellationToken cancellationToken = default);

    Task<List<AssetUnit>> GetAssetUnitsByAssetIdAsync(Guid assetId, CancellationToken cancellationToken = default);
}
