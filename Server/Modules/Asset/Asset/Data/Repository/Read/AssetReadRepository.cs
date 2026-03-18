using Microsoft.EntityFrameworkCore;
using Shared.Data;

namespace Asset.Data.Repository.Read;

public class AssetReadRepository(AssetDbContext dbContext)
    : BaseReadRepository<Assets.Model.Asset, Guid>(dbContext), IAssetReadRepository
{
    private readonly AssetDbContext _context = dbContext;

    public async Task<bool> NameExistsInLaboratoryAsync(
        Guid laboratoryId,
        string name,
        Guid? excludeAssetModelId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Laboratories
            .AsNoTracking()
            .Where(x => x.Id == laboratoryId);

        if (excludeAssetModelId.HasValue)
        {
            query = query.Where(x => x.Id != excludeAssetModelId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
