using Shared.Data.Repository;

namespace Asset.Data.Repository;

public interface IAssetRepository : IRepository<Assets.Model.Asset, Guid>
{
    Task<bool> NameExistsInLaboratoryAsync(Guid laboratoryId, string name, Guid? excludeAssetModelId = null, CancellationToken cancellationToken = default);
    Task SyncAvailabilityAsync(Guid assetModelId, CancellationToken cancellationToken = default);
}
