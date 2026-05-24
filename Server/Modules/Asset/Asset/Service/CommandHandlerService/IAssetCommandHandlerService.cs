using Asset.Assets.Features.AssetFeature.CreateAsset;
using Shared.Pagination;

namespace Asset.Service.CommandHandlerService;

public interface IAssetCommandHandlerService
{
    Task<Guid> CreateAsset(AssetDto asset, List<Guid> units, CancellationToken cancellationToken);
    Task<bool> AssignAssetUnits(Assets.Model.Asset asset, List<Guid> units);
    Task<bool> AssignAssetUnitsAsync(Guid assetId, List<Guid> units, CancellationToken cancellationToken);
    Task<bool> ReserveAssetsAsync(List<Guid> assetIds, Guid performedBy, Guid? requestId, CancellationToken cancellationToken);
    Task<bool> MarkAssetsInUseAsync(List<Guid> assetIds, Guid responsibleUserId, Guid performedBy, Guid? requestId, CancellationToken cancellationToken);
    Task<bool> ReleaseAssetsAsync(List<Guid> assetIds, Guid performedBy, Guid? requestId, string? remark, CancellationToken cancellationToken);
    Task<bool> ReturnVisibleAssetAsync(Guid assetId, Guid userId, string roleCode, CancellationToken cancellationToken);
    Task<bool> AssignAssetsToLaboratoryAsync(Guid laboratoryId, List<Guid> assetIds, Guid performedBy, Guid? requestId, CancellationToken cancellationToken);
    Task<AssetDto> GetAssetById(Guid assetId, CancellationToken cancellationToken);
    Task<AssetDto> GetVisibleAssetById(Guid assetId, Guid userId, string roleCode, CancellationToken cancellationToken);
    Task<bool> UpdateAsset(Guid assetId, AssetDto asset, CancellationToken cancellationToken);
    Task<bool> DeleteAsset(Guid assetId, CancellationToken cancellationToken);
    Task<PaginatedResult<AssetDto>> GetAssets(PaginationRequest paginationRequest, string? searchTerm = null, CancellationToken cancellationToken = default);
    Task<PaginatedResult<AssetDto>> GetVisibleAssets(
        PaginationRequest paginationRequest,
        Guid userId,
        string roleCode,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
    Task<PaginatedResult<AssetDto>> GetAssetsByLab(Guid laboratoryId, PaginationRequest paginationRequest, string? searchTerm = null, CancellationToken cancellationToken = default);
    Task<PaginatedResult<AssetDto>> GetVisibleAssetsByLab(
        Guid laboratoryId,
        PaginationRequest paginationRequest,
        Guid userId,
        string roleCode,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
    Task<PaginatedResult<AssetDto>> GetAllocatableAssets(
        PaginationRequest paginationRequest,
        string roleCode,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
    Task<long> GetAssetCount(CancellationToken cancellationToken = default);
    Task<long> GetVisibleAssetCount(Guid userId, string roleCode, CancellationToken cancellationToken = default);
    Task<long> GetAssetCountByLab(Guid laboratoryId, CancellationToken cancellationToken = default);
}
