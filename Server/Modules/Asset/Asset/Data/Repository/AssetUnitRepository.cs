using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Repository;

namespace Asset.Data.Repository;

public class AssetUnitRepository(AssetDbContext dbContext)
    : Repository<AssetUnit, Guid>(dbContext), IAssetUnitRepository
{
    private readonly AssetDbContext _context = dbContext;

    public async Task<List<AssetUnitDto>> GetAssetUnitsByAssetIdAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .AsNoTracking()
            .Where(x => EF.Property<Guid>(x, "AssetId") == assetId)
            .Select(x => new AssetUnitDto
            {
                Id = x.Id,
                AssetId = EF.Property<Guid>(x, "AssetId"),
                AssetTag = x.AssetTag,
                SerialNo = x.SerialNo,
                Name = x.Name,
                Brand = x.Brand,
                AvailabilityStatus = x.AvailabilityStatus,
                OperationalStatus = x.OperationalStatus,
                Remark = x.Remark,
                OwnerId = x.OwnerId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AssetUnitDto?> GetAssetUnitDtoByIdAsync(Guid assetUnitId, CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .AsNoTracking()
            .Where(x => x.Id == assetUnitId)
            .Select(x => new AssetUnitDto
            {
                Id = x.Id,
                AssetId = EF.Property<Guid>(x, "AssetId"),
                AssetTag = x.AssetTag,
                SerialNo = x.SerialNo,
                Name = x.Name,
                Brand = x.Brand,
                AvailabilityStatus = x.AvailabilityStatus,
                OperationalStatus = x.OperationalStatus,
                Remark = x.Remark,
                OwnerId = x.OwnerId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AssetUnit?> GetByIdWithHistoriesAsync(Guid assetUnitId, CancellationToken cancellationToken = default)
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
