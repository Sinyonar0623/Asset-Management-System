using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data;

namespace Asset.Data.Repository.Read;

public class LaboratoryReadRepository(AssetDbContext dbContext)
    : BaseReadRepository<Laboratory, Guid>(dbContext), ILaboratoryReadRepository
{
    private readonly AssetDbContext _context = dbContext;

    public async Task<bool> HasAssetModelsAsync(Guid laboratoryId, CancellationToken cancellationToken = default)
    {
        return await _context.AssetModels
            .AsNoTracking()
            .AnyAsync(x => x.Id == laboratoryId, cancellationToken);
    }
}
