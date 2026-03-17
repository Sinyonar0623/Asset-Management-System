using Asset.Assets.Model;
using Asset.Data.Repository;

namespace Asset.Service;

public class AssetUnitService(IAssetUnitRepository assetUnitRepository, IAssetRepository assetRepository) : IAssetUnitService
{
    private readonly IAssetUnitRepository _assetUnitRepository = assetUnitRepository;
    private readonly IAssetRepository _assetRepository = assetRepository;

    public async Task<List<AssetUnitDto>> GetAssetUnitsByAssetId(Guid assetId, CancellationToken cancellationToken = default)
    {
        if (assetId == Guid.Empty)
        {
            throw new ArgumentException("Asset id is required.", nameof(assetId));
        }

        return await _assetUnitRepository.GetAssetUnitsByAssetIdAsync(assetId, cancellationToken);
    }

    public async Task<AssetUnitDto> GetAssetUnit(Guid assetUnitId, CancellationToken cancellationToken = default)
    {
        var assetUnit = await _assetUnitRepository.GetAssetUnitDtoByIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        return assetUnit;
    }

    public async Task<List<Guid>> CreateAssetUnit(List<AssetUnitDto> assetUnits, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assetUnits);
        if (assetUnits.Count == 0)
        {
            return [];
        }

        var assetIds = assetUnits.Select(x => x.AssetId).Distinct().ToList();
        var assetsById = new Dictionary<Guid, Assets.Model.Asset>();
        foreach (var assetId in assetIds)
        {
            var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken)
                ?? throw new KeyNotFoundException($"Asset model with id {assetId} was not found.");

            assetsById[assetId] = asset;
        }

        var newAssetUnits = new List<AssetUnit>(assetUnits.Count);
        foreach (var unit in assetUnits)
        {
            var newAssetUnit = AssetUnit.Create(
                unit.AssetTag,
                unit.SerialNo,
                unit.Name,
                unit.Brand,
                string.IsNullOrWhiteSpace(unit.AvailabilityStatus)
                    ? AssetUnitStatuses.Availability.PendingActivation
                    : unit.AvailabilityStatus,
                string.IsNullOrWhiteSpace(unit.OperationalStatus)
                    ? AssetUnitStatuses.Operational.Ready
                    : unit.OperationalStatus,
                unit.Remark,
                unit.OwnerId
            );
            newAssetUnit.AssignAssets(assetsById[unit.AssetId]);
            newAssetUnits.Add(newAssetUnit);
        }

        await _assetUnitRepository.AddRangeAsync(newAssetUnits, cancellationToken);
        await _assetUnitRepository.SaveChangeAsync(cancellationToken);

        foreach (var assetId in assetIds)
        {
            await _assetRepository.SyncAvailabilityAsync(assetId, cancellationToken);
        }
        await _assetRepository.SaveChangeAsync(cancellationToken);

        return newAssetUnits.Select(x => x.Id).ToList();
    }

    public async Task<Guid> UpdateAssetUnit(Guid assetUnitId, AssetUnitDto assetUnit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assetUnit);

        var currentAssetUnit = await _assetUnitRepository.GetAssetUnitDtoByIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        if (await _assetUnitRepository.AssetTagExistsAsync(assetUnit.AssetTag, assetUnitId, cancellationToken))
        {
            throw new InvalidOperationException($"Asset tag '{assetUnit.AssetTag}' already exists.");
        }

        if (await _assetUnitRepository.SerialNoExistsAsync(assetUnit.SerialNo, assetUnitId, cancellationToken))
        {
            throw new InvalidOperationException($"Serial no '{assetUnit.SerialNo}' already exists.");
        }

        var asset = await _assetRepository.GetByIdAsync(assetUnit.AssetId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset model with id {assetUnit.AssetId} was not found.");

        var entity = await _assetUnitRepository.GetByIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        entity.Update(
            assetUnit.AssetTag,
            assetUnit.SerialNo,
            assetUnit.Name,
            assetUnit.Brand,
            assetUnit.AvailabilityStatus,
            assetUnit.OperationalStatus,
            assetUnit.Remark,
            assetUnit.OwnerId);

        entity.AssignAssets(asset);

        await _assetUnitRepository.SaveChangeAsync(cancellationToken);

        await _assetRepository.SyncAvailabilityAsync(currentAssetUnit.AssetId, cancellationToken);

        if (currentAssetUnit.AssetId != assetUnit.AssetId)
        {
            await _assetRepository.SyncAvailabilityAsync(assetUnit.AssetId, cancellationToken);
        }

        await _assetRepository.SaveChangeAsync(cancellationToken);

        return assetUnitId;
    }

    public async Task<Guid> DeleteAssetUnit(Guid assetUnitId, CancellationToken cancellationToken = default)
    {
        var currentAssetUnit = await _assetUnitRepository.GetAssetUnitDtoByIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        var entity = await _assetUnitRepository.GetByIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        await _assetUnitRepository.DeleteAsync(entity, cancellationToken);
        await _assetUnitRepository.SaveChangeAsync(cancellationToken);

        await _assetRepository.SyncAvailabilityAsync(currentAssetUnit.AssetId, cancellationToken);
        await _assetRepository.SaveChangeAsync(cancellationToken);

        return assetUnitId;
    }
}
