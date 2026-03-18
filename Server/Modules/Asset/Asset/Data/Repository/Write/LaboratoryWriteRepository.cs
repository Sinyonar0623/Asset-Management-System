using Asset.Assets.Model;
using Shared.Data;

namespace Asset.Data.Repository.Write;

public class LaboratoryWriteRepository(AssetDbContext dbContext)
    : BaseRepository<Laboratory, Guid>(dbContext), ILaboratoryWriteRepository
{
}
