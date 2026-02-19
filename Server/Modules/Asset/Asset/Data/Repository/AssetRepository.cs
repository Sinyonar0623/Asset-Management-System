using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Repository;

namespace Asset.Data.Repository;

public class AssetRepository(AssetDbContext dbContext)
    : Repository<Assets.Model.Asset, Guid>(dbContext), IAssetRepository
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
            .Where(x => x.Id == laboratoryId );

        if (excludeAssetModelId.HasValue)
        {
            query = query.Where(x => x.Id != excludeAssetModelId.Value);
        }

        return await query.AnyAsync(cancellationToken);
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
                x => x.Id == assetModelId
                     && x.AvailabilityStatus == "AVAILABLE"
                     && x.OperationalStatus == "READY",
                cancellationToken);

        assetModel.SetAvailability(isAvailable);
    }
}
