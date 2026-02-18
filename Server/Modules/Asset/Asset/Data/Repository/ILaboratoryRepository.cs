using Asset.Assets.Model;
using Shared.Data.Repository;

namespace Asset.Data.Repository;

public interface ILaboratoryRepository : IRepository<Laboratory, long>
{
    Task<bool> HasAssetModelsAsync(long laboratoryId, CancellationToken cancellationToken = default);
}
