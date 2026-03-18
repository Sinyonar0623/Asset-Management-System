namespace Asset.Service.CommandHandlerService;

public interface IAssetUnitCommandHandlerService
{
    Task<List<Guid>> CreateAssetUnit(List<AssetUnitDto> assetUnits, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAssetUnit(Guid assetUnitId, AssetUnitDto assetUnit, CancellationToken cancellationToken = default);
    Task<Guid> DeleteAssetUnit(Guid assetUnitId, CancellationToken cancellationToken = default);
    Task<List<AssetUnitDto>> GetAssetUnitsByAssetId(Guid assetId, CancellationToken cancellationToken = default);
    Task<AssetUnitDto> GetAssetUnitById(Guid assetUnitId, CancellationToken cancellationToken = default);
}
