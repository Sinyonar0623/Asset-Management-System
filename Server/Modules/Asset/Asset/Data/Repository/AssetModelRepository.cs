using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Repository;

namespace Asset.Data.Repository;

public class AssetModelRepository(AssetDbContext dbContext)
    : Repository<AssetModel, long>(dbContext), IAssetModelRepository
{
    private readonly AssetDbContext _context = dbContext;

    public async Task<bool> NameExistsInLaboratoryAsync(
        long laboratoryId,
        string name,
        long? excludeAssetModelId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AssetModels
            .AsNoTracking()
            .Where(x => x.LaboratoryId == laboratoryId && x.Name == name);

        if (excludeAssetModelId.HasValue)
        {
            query = query.Where(x => x.Id != excludeAssetModelId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasAssetUnitsAsync(long assetModelId, CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .AsNoTracking()
            .AnyAsync(x => x.AssetModelId == assetModelId, cancellationToken);
    }

    public async Task SyncAvailabilityAsync(long assetModelId, CancellationToken cancellationToken = default)
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
                x => x.AssetModelId == assetModelId
                     && x.AvailabilityStatus == "AVAILABLE"
                     && x.OperationalStatus == "READY",
                cancellationToken);

        assetModel.SetAvailability(isAvailable);
    }
}
