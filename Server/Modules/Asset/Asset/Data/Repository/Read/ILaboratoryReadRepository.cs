using Asset.Assets.Model;
using Shared.Data;

namespace Asset.Data.Repository.Read;

public interface ILaboratoryReadRepository : IReadRepository<Laboratory, Guid>
{
    Task<bool> HasAssetModelsAsync(Guid laboratoryId, CancellationToken cancellationToken = default);
}
