using Asset.Assets.Model;
using Shared.Data.Repository;

namespace Asset.Data.Repository;

public interface IAssetModelRepository : IRepository<AssetModel, long>
{
    Task<bool> NameExistsInLaboratoryAsync(
        long laboratoryId,
        string name,
        long? excludeAssetModelId = null,
        CancellationToken cancellationToken = default);
    Task<bool> HasAssetUnitsAsync(long assetModelId, CancellationToken cancellationToken = default);
    Task SyncAvailabilityAsync(long assetModelId, CancellationToken cancellationToken = default);
}
