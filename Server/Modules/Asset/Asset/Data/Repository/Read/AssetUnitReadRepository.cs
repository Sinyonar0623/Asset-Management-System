using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data;

namespace Asset.Data.Repository.Read;

public class AssetUnitReadRepository(AssetDbContext dbContext)
    : BaseReadRepository<AssetUnit, Guid>(dbContext), IAssetUnitReadRepository
{
    private readonly AssetDbContext _context = dbContext;

    public async Task<List<AssetUnitDto>> GetAssetUnitsByAssetIdAsync(
        Guid assetId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .AsNoTracking()
            .Where(x => EF.Property<Guid?>(x, "AssetId") == assetId)
            .Select(x => new AssetUnitDto
            {
                Id = x.Id,
                AssetId = EF.Property<Guid?>(x, "AssetId"),
                AssetTag = x.AssetTag,
                SerialNo = x.SerialNo,
                Name = x.Name,
                Brand = x.Brand,
                AvailabilityStatus = x.AvailabilityStatus,
                OperationalStatus = x.OperationalStatus,
                Remark = x.Remark,
                ResponsibleUserId = x.ResponsibleUserId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AssetUnitDto?> GetAssetUnitDtoByIdAsync(
        Guid assetUnitId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .AsNoTracking()
            .Where(x => x.Id == assetUnitId)
            .Select(x => new AssetUnitDto
            {
                Id = x.Id,
                AssetId = EF.Property<Guid?>(x, "AssetId"),
                AssetTag = x.AssetTag,
                SerialNo = x.SerialNo,
                Name = x.Name,
                Brand = x.Brand,
                AvailabilityStatus = x.AvailabilityStatus,
                OperationalStatus = x.OperationalStatus,
                Remark = x.Remark,
                ResponsibleUserId = x.ResponsibleUserId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
