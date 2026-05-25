using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data;

namespace Asset.Data.Repository.Write;

public class AssetWriteRepository(AssetDbContext dbContext)
    : BaseRepository<Assets.Model.Asset, Guid>(dbContext), IAssetWriteRepository
{
    private readonly AssetDbContext _context = dbContext;

    public override async Task<Assets.Model.Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AssetModels
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task SyncAvailabilityAsync(Guid assetModelId, CancellationToken cancellationToken = default)
    {
        var assetModel = await _context.AssetModels
            .FirstOrDefaultAsync(x => x.Id == assetModelId, cancellationToken);

        if (assetModel is null)
        {
            return;
        }

        var isAvailable = await _context.AssetUnits
            .AsNoTracking()
            .AnyAsync(
                x => EF.Property<Guid?>(x, "AssetId") == assetModelId
                     && x.AvailabilityStatus == AssetUnitStatuses.Availability.Available
                     && x.OperationalStatus == AssetUnitStatuses.Operational.Ready,
                cancellationToken);

        assetModel.SetAvailability(isAvailable);
    }

    public async Task<bool> TryReserveAsync(
        Guid assetId,
        Guid performedBy,
        Guid? requestId = null,
        CancellationToken cancellationToken = default)
    {
        var asset = await _context.AssetModels
            .FirstOrDefaultAsync(x => x.Id == assetId, cancellationToken);

        if (asset is null || !asset.IsAvailable)
        {
            return false;
        }

        var assetUnits = await _context.AssetUnits
            .Include(x => x.Histories)
            .Where(x => EF.Property<Guid?>(x, "AssetId") == assetId
                        && x.AvailabilityStatus == AssetUnitStatuses.Availability.Available
                        && x.OperationalStatus == AssetUnitStatuses.Operational.Ready)
            .ToListAsync(cancellationToken);

        if (assetUnits.Count == 0)
        {
            return false;
        }

        foreach (var assetUnit in assetUnits)
        {
            assetUnit.Reserve(performedBy, requestId);
        }

        asset.SetAvailability(false);

        return true;
    }

    public async Task<bool> TryMarkInUseAsync(
        Guid assetId,
        Guid responsibleUserId,
        Guid performedBy,
        Guid? requestId = null,
        CancellationToken cancellationToken = default)
    {
        var asset = await _context.AssetModels
            .FirstOrDefaultAsync(x => x.Id == assetId, cancellationToken);

        var assetUnits = await _context.AssetUnits
            .Include(x => x.Histories)
            .Where(x => EF.Property<Guid?>(x, "AssetId") == assetId)
            .ToListAsync(cancellationToken);

        if (asset is null || assetUnits.Count == 0)
        {
            return false;
        }

        foreach (var assetUnit in assetUnits)
        {
            assetUnit.MarkInUse(responsibleUserId, performedBy, requestId);
        }

        asset?.SetAvailability(false);

        return true;
    }

    public async Task<bool> TryReleaseAsync(
        Guid assetId,
        Guid performedBy,
        Guid? requestId = null,
        string? remark = null,
        string actionType = "RELEASE",
        CancellationToken cancellationToken = default)
    {
        var asset = await _context.AssetModels
            .FirstOrDefaultAsync(x => x.Id == assetId, cancellationToken);

        var assetUnits = await _context.AssetUnits
            .Include(x => x.Histories)
            .Where(x => EF.Property<Guid?>(x, "AssetId") == assetId)
            .ToListAsync(cancellationToken);

        if (asset is null || assetUnits.Count == 0)
        {
            return false;
        }

        foreach (var assetUnit in assetUnits)
        {
            if (assetUnit.AvailabilityStatus == AssetUnitStatuses.Availability.Available
                && assetUnit.OperationalStatus == AssetUnitStatuses.Operational.Ready
                && assetUnit.ResponsibleUserId is null)
            {
                continue;
            }

            assetUnit.Release(performedBy, requestId, remark, actionType);
        }

        asset?.SetAvailability(true);

        return true;
    }
}
