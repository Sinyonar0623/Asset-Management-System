using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Repository;

namespace Asset.Data.Repository;

public class AssetUnitRepository(AssetDbContext dbContext)
    : Repository<AssetUnit, Guid>(dbContext), IAssetUnitRepository
{
    private readonly AssetDbContext _context = dbContext;

    public async Task<AssetUnit?> GetByIdWithHistoriesAsync(Guid assetUnitId, CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .Include(x => x.Histories)
            .Include(x => x.Condition)
            .FirstOrDefaultAsync(x => x.Id == assetUnitId, cancellationToken);
    }

    public async Task<bool> AssetTagExistsAsync(
        string assetTag,
        Guid? excludeAssetUnitId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AssetUnits
            .AsNoTracking()
            .Where(x => x.AssetTag == assetTag);

        if (excludeAssetUnitId.HasValue)
        {
            query = query.Where(x => x.Id != excludeAssetUnitId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> SerialNoExistsAsync(
        string serialNo,
        Guid? excludeAssetUnitId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AssetUnits
            .AsNoTracking()
            .Where(x => x.SerialNo == serialNo);

        if (excludeAssetUnitId.HasValue)
        {
            query = query.Where(x => x.Id != excludeAssetUnitId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
