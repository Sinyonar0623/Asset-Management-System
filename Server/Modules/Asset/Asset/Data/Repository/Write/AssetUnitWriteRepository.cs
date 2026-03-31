using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data;

namespace Asset.Data.Repository.Write;

public class AssetUnitWriteRepository(AssetDbContext dbContext)
    : BaseRepository<AssetUnit, Guid>(dbContext), IAssetUnitWriteRepository
{
    private readonly AssetDbContext _context = dbContext;

    public override async Task<AssetUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<AssetUnit?> GetByIdWithHistoriesAsync(
        Guid assetUnitId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .Include(x => x.Histories)
            .Include(x => x.Condition)
            .Include(x => x.Images)
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
            query = query.Where(x => x.Id != excludeAssetUnitId.Value);

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
            query = query.Where(x => x.Id != excludeAssetUnitId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<List<AssetUnit>> GetAssetUnitsByIdsAsync(
        IReadOnlyCollection<Guid> assetUnitIds,
        CancellationToken cancellationToken = default)
    {
        if (assetUnitIds.Count == 0)
        {
            return [];
        }

        return await _context.AssetUnits
            .Where(x => assetUnitIds.Contains(x.Id))
            .Include(x => x.Asset)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AssetUnit>> GetAssetUnitsByAssetIdAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        var assetUnits = await _context.AssetUnits
            .Where(x => EF.Property<Guid?>(x, "AssetId") == assetId)
            .ToListAsync(cancellationToken);

        return assetUnits;
    }

}
