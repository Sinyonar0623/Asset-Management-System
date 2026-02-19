using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Repository;

namespace Asset.Data.Repository;

public class LaboratoryRepository(AssetDbContext dbContext)
    : Repository<Laboratory, Guid>(dbContext), ILaboratoryRepository
{
    private readonly AssetDbContext _context = dbContext;

    public async Task<bool> HasAssetModelsAsync(Guid laboratoryId, CancellationToken cancellationToken = default)
    {
        return await _context.AssetModels
            .AsNoTracking()
            .AnyAsync(x => x.Id == laboratoryId, cancellationToken);
    }
}
