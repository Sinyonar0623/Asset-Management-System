using Shared.Data;

namespace Asset.Data.Repository.Write;

public interface IAssetWriteRepository : IRepository<Assets.Model.Asset, Guid>
{
    Task SyncAvailabilityAsync(Guid assetModelId, CancellationToken cancellationToken = default);
    Task<bool> TryReserveAsync(Guid assetId, CancellationToken cancellationToken = default);
    Task<bool> TryReleaseAsync(Guid assetId, CancellationToken cancellationToken = default);
}
