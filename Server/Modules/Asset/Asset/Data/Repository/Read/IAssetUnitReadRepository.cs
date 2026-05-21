using Asset.Assets.Model;
using Shared.Data;

namespace Asset.Data.Repository.Read;

public interface IAssetUnitReadRepository : IReadRepository<AssetUnit, Guid>
{
    Task<List<AssetUnitDto>> GetAssetUnitsByAssetIdAsync(
        Guid assetId,
        CancellationToken cancellationToken = default);

    Task<List<AssetUnitDto>> GetUnassignedAssetUnitsAsync(
        CancellationToken cancellationToken = default);

    Task<AssetUnitDto?> GetAssetUnitDtoByIdAsync(
        Guid assetUnitId,
        CancellationToken cancellationToken = default);

    Task<AssetUnitDetailDto?> GetAssetUnitDetailDtoByIdAsync(
        Guid assetUnitId,
        CancellationToken cancellationToken = default);

    Task<List<AssetHistoryDto>> GetAssetHistoriesByAssetIdAsync(
        Guid assetId,
        CancellationToken cancellationToken = default);
}
