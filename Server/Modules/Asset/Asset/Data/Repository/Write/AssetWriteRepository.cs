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
}
