using Asset.Assets.Model;
using Asset.Data;
using Asset.Data.Repository.Read;
using Asset.Data.Repository.Write;
using Shared.Data.UnitOfWork;

namespace Asset.Service.CommandHandlerService;

public class AssetUnitCommandHandlerService(
    IAssetUnitReadRepository assetUnitReadRepository,
    IAssetUnitWriteRepository assetUnitWriteRepository,
    IAssetWriteRepository assetWriteRepository,
    IUnitOfWork<AssetDbContext> unitOfWork) : IAssetUnitCommandHandlerService
{
    private readonly IAssetUnitReadRepository _assetUnitReadRepository = assetUnitReadRepository;
    private readonly IAssetUnitWriteRepository _assetUnitWriteRepository = assetUnitWriteRepository;
    private readonly IAssetWriteRepository _assetWriteRepository = assetWriteRepository;
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;

    public async Task<List<Guid>> CreateAssetUnit(List<AssetUnitDto> assetUnits, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assetUnits);

        if (assetUnits.Count == 0) return [];

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

            if (unit.AssetId.HasValue && unit.AssetId.Value != Guid.Empty)
            {
                var asset = await _assetWriteRepository.GetByIdAsync(unit.AssetId.Value, cancellationToken)
                    ?? throw new KeyNotFoundException($"Asset with id {unit.AssetId.Value} was not found.");

                newAssetUnit.AssignAsset(asset);
            }
            
            newAssetUnits.Add(newAssetUnit);
        }

        await _assetUnitWriteRepository.AddRangeAsync(newAssetUnits, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return [.. newAssetUnits.Select(x => x.Id)];
    }

    public async Task<Guid> UpdateAssetUnit(Guid assetUnitId, AssetUnitDto assetUnit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assetUnit);

        var currentAssetUnit = await _assetUnitReadRepository.GetAssetUnitDtoByIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        if (await _assetUnitWriteRepository.AssetTagExistsAsync(assetUnit.AssetTag, assetUnitId, cancellationToken))
            throw new InvalidOperationException($"Asset tag '{assetUnit.AssetTag}' already exists.");

        if (await _assetUnitWriteRepository.SerialNoExistsAsync(assetUnit.SerialNo, assetUnitId, cancellationToken))
            throw new InvalidOperationException($"Serial no '{assetUnit.SerialNo}' already exists.");

        var entity = await _assetUnitWriteRepository.GetByIdAsync(assetUnitId, cancellationToken)
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

        if (assetUnit.AssetId.HasValue && assetUnit.AssetId.Value != Guid.Empty)
        {
            var asset = await _assetWriteRepository.GetByIdAsync(assetUnit.AssetId.Value, cancellationToken)
                ?? throw new KeyNotFoundException($"Asset with id {assetUnit.AssetId.Value} was not found.");

            entity.AssignAsset(asset);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return assetUnitId;
    }

    public async Task<Guid> DeleteAssetUnit(Guid assetUnitId, CancellationToken cancellationToken = default)
    {
        var currentAssetUnit = await _assetUnitReadRepository.GetAssetUnitDtoByIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        await _assetUnitWriteRepository.DeleteAsync(assetUnitId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return assetUnitId;
    }

    public async Task<List<AssetUnitDto>> GetAssetUnitsByAssetId(Guid assetId, CancellationToken cancellationToken = default)
    {
        if (assetId == Guid.Empty)
            throw new ArgumentException("Asset id is required.", nameof(assetId));

        return await _assetUnitReadRepository.GetAssetUnitsByAssetIdAsync(assetId, cancellationToken);
    }

    public async Task<AssetUnitDto> GetAssetUnitById(Guid assetUnitId, CancellationToken cancellationToken = default)
    {
        var assetUnit = await _assetUnitReadRepository.GetAssetUnitDtoByIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        return assetUnit;
    }

}
