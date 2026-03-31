using Asset.Assets.Model;
using Asset.Data.Repository.Read;
using Asset.Data.Repository.Write;

namespace Asset.Service.EventHandlerService;

public class AssetUnitEventHandlerService(
    IAssetUnitWriteRepository assetUnitWriteRepository,
    IAssetUnitReadRepository assetUnitReadRepository
    ) : IAssetUnitEventHandlerService
{
    private readonly IAssetUnitWriteRepository _assetUnitWriteRepository = assetUnitWriteRepository;
    private readonly IAssetUnitReadRepository _assetUnitReadRepository = assetUnitReadRepository;

    public async Task<bool> AssignAssetUnit(Assets.Model.Asset asset, List<Guid> assetUnitId, CancellationToken cancellationToken)
    {
        if (asset is null)
            throw new KeyNotFoundException($"Asset was not found.");

        if (assetUnitId is null || assetUnitId.Count == 0)
            return true;

        var requestedUnitIds = assetUnitId
            .Distinct()
            .ToList();

        var units = await _assetUnitWriteRepository.GetAssetUnitsByIdsAsync(requestedUnitIds, cancellationToken);

        if (units.Count != requestedUnitIds.Count)
            throw new KeyNotFoundException($"New asset unit was not found.");

        foreach (var unit in units)
        {
            if (unit.Asset is not null)
                throw new KeyNotFoundException($"Asset unit with id was conflict.");

            if (!AssetUnitStatuses.IsReadyForAssignAsset(unit.AvailabilityStatus, unit.OperationalStatus))
                throw new KeyNotFoundException($"Asset unit with id was conflict.");

            unit.AssignAsset(asset);
        }

        return true;
    }


    public async Task<AssetUnit?> GetAssetUnitById(Guid assetUnitId, CancellationToken cancellationToken = default)
    {
        var assetUnit = await _assetUnitWriteRepository.GetByIdAsync(assetUnitId, cancellationToken);

        return assetUnit;
    }

}
