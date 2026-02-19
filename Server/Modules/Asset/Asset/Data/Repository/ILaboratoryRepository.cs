using Asset.Assets.Model;
using Shared.Data.Repository;

namespace Asset.Data.Repository;

public interface ILaboratoryRepository : IRepository<Laboratory, Guid>
{
    Task<bool> HasAssetModelsAsync(Guid laboratoryId, CancellationToken cancellationToken = default);
}
