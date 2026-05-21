using Shared.Data;

namespace Asset.Data.Repository.Read;

public interface IAssetReadRepository : IReadRepository<Assets.Model.Asset, Guid>
{
    Task<long> GetAssetCountAsync(CancellationToken cancellationToken = default);
    Task<long> GetAssetCountByLaboratoryIdAsync(Guid laboratoryId, CancellationToken cancellationToken = default);
    Task<bool> NameExistsInLaboratoryAsync(
        Guid laboratoryId,
        string name,
        Guid? excludeAssetModelId = null,
        CancellationToken cancellationToken = default);

    Task<List<Guid>> GetRequestedAssetIdsByRequesterAsync(
        Guid requesterId,
        CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, string>> GetAvailabilityStatusesByAssetIdsAsync(
        IReadOnlyCollection<Guid> assetIds,
        CancellationToken cancellationToken = default);
}
