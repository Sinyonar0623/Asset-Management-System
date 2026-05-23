namespace Asset.Service.CommandHandlerService;

public interface IAssetUnitCommandHandlerService
{
    Task<List<Guid>> CreateAssetUnit(List<AssetUnitDto> assetUnits, CancellationToken cancellationToken = default);
    Task<bool> UpdateAssetUnit(Guid assetUnitId, AssetUnitDto assetUnit, CancellationToken cancellationToken = default);
    Task<bool> DeleteAssetUnit(Guid assetUnitId, CancellationToken cancellationToken = default);
    Task<List<AssetUnitDto>> GetAssetUnitsByAssetId(Guid assetId, CancellationToken cancellationToken = default);
    Task<List<AssetUnitDto>> GetUnassignedAssetUnits(CancellationToken cancellationToken = default);
    Task<AssetUnitDto> GetAssetUnitById(Guid assetUnitId, CancellationToken cancellationToken = default);
    Task<AssetUnitDetailDto> GetAssetUnitDetailById(Guid assetUnitId, CancellationToken cancellationToken = default);
    Task<List<AssetUnitImageDto>> GetAssetUnitImagesByAssetUnitId(Guid assetUnitId, CancellationToken cancellationToken = default);
    Task<List<AssetUnitImageDto>> AddAssetUnitImages(
        Guid assetUnitId,
        List<CreateAssetUnitImageDto> images,
        CancellationToken cancellationToken = default);
}
