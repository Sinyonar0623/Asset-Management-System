using Asset.Assets.Features.AssetFeature.CreateAsset;
using Shared.Pagination;

namespace Asset.Service.CommandHandlerService;

public interface IAssetCommandHandlerService
{
    Task<Guid> CreateAsset(AssetDto asset, List<Guid> units, CancellationToken cancellationToken);
    Task<bool> AssignAssetUnits(Assets.Model.Asset asset, List<Guid> units);
    Task<bool> AssignAssetUnitsAsync(Guid assetId, List<Guid> units, CancellationToken cancellationToken);
    Task<bool> ReserveAssetsAsync(List<Guid> assetIds, CancellationToken cancellationToken);
    Task<bool> ReleaseAssetsAsync(List<Guid> assetIds, CancellationToken cancellationToken);
    Task<AssetDto> GetAssetById(Guid assetId, CancellationToken cancellationToken);
    Task<bool> UpdateAsset(Guid assetId, AssetDto asset, CancellationToken cancellationToken);
    Task<bool> DeleteAsset(Guid assetId, CancellationToken cancellationToken);
    Task<PaginatedResult<AssetDto>> GetAssets(PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
    Task<PaginatedResult<AssetDto>> GetAssetsByLab(Guid laboratoryId, PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
    Task<long> GetAssetCount(CancellationToken cancellationToken = default);
    Task<long> GetAssetCountByLab(Guid laboratoryId, CancellationToken cancellationToken = default);
}
