using Shared.Data;

namespace Asset.Data.Repository.Write;

public interface IAssetWriteRepository : IRepository<Assets.Model.Asset, Guid>
{
    Task SyncAvailabilityAsync(Guid assetModelId, CancellationToken cancellationToken = default);
    Task<bool> TryReserveAsync(
        Guid assetId,
        Guid performedBy,
        Guid? requestId = null,
        CancellationToken cancellationToken = default);
    Task<bool> TryMarkInUseAsync(
        Guid assetId,
        Guid responsibleUserId,
        Guid performedBy,
        Guid? requestId = null,
        CancellationToken cancellationToken = default);
    Task<bool> TryReleaseAsync(
        Guid assetId,
        Guid performedBy,
        Guid? requestId = null,
        string? remark = null,
        string actionType = "RELEASE",
        CancellationToken cancellationToken = default);
}
