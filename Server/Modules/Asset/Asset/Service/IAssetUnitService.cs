namespace Asset.Service;

public interface IAssetUnitService
{
    Task<List<AssetUnitDto>> GetAssetUnitsByAssetId(Guid assetId, CancellationToken cancellationToken = default);
    Task<AssetUnitDto> GetAssetUnit(Guid assetUnitId, CancellationToken cancellationToken = default);
    Task<List<Guid>> CreateAssetUnit(List<AssetUnitDto> assetUnits, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAssetUnit(Guid assetUnitId, AssetUnitDto assetUnit, CancellationToken cancellationToken = default);
    Task<Guid> DeleteAssetUnit(Guid assetUnitId, CancellationToken cancellationToken = default);
}
