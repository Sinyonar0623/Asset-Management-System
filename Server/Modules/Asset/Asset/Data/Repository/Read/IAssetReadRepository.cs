using Shared.Data;

namespace Asset.Data.Repository.Read;

public interface IAssetReadRepository : IReadRepository<Assets.Model.Asset, Guid>
{
    Task<bool> NameExistsInLaboratoryAsync(
        Guid laboratoryId,
        string name,
        Guid? excludeAssetModelId = null,
        CancellationToken cancellationToken = default);
}
