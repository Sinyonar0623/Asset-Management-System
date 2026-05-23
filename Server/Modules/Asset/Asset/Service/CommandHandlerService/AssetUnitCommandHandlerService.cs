using Asset.Assets.Model;
using Asset.Data.Repository.Read;
using Asset.Data.Repository.Write;

namespace Asset.Service.CommandHandlerService;

public class AssetUnitCommandHandlerService(
    IAssetUnitReadRepository assetUnitReadRepository,
    IAssetUnitWriteRepository assetUnitWriteRepository,
    IAssetWriteRepository assetWriteRepository) : IAssetUnitCommandHandlerService
{
    private readonly IAssetUnitReadRepository _assetUnitReadRepository = assetUnitReadRepository;
    private readonly IAssetUnitWriteRepository _assetUnitWriteRepository = assetUnitWriteRepository;
    private readonly IAssetWriteRepository _assetWriteRepository = assetWriteRepository;

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
                unit.ResponsibleUserId
            );

            newAssetUnit.AddHistory(
                "CREATE",
                "Asset unit created.",
                Guid.Empty,
                toAvailabilityStatus: newAssetUnit.AvailabilityStatus,
                toOperationalStatus: newAssetUnit.OperationalStatus,
                toResponsibleUserId: newAssetUnit.ResponsibleUserId);

            if (unit.AssetId.HasValue && unit.AssetId.Value != Guid.Empty)
            {
                var asset = await _assetWriteRepository.GetByIdAsync(unit.AssetId.Value, cancellationToken)
                    ?? throw new KeyNotFoundException($"Asset with id {unit.AssetId.Value} was not found.");

                newAssetUnit.AssignAsset(asset);
                newAssetUnit.AddHistory(
                    "ASSIGN_ASSET",
                    "Asset unit assigned to asset.",
                    Guid.Empty,
                    fromAvailabilityStatus: newAssetUnit.AvailabilityStatus,
                    toAvailabilityStatus: newAssetUnit.AvailabilityStatus,
                    fromOperationalStatus: newAssetUnit.OperationalStatus,
                    toOperationalStatus: newAssetUnit.OperationalStatus,
                    fromResponsibleUserId: newAssetUnit.ResponsibleUserId,
                    toResponsibleUserId: newAssetUnit.ResponsibleUserId);
            }
            
            newAssetUnits.Add(newAssetUnit);
        }

        await _assetUnitWriteRepository.AddRangeAsync(newAssetUnits, cancellationToken);

        return [.. newAssetUnits.Select(x => x.Id)];
    }

    public async Task<bool> UpdateAssetUnit(Guid assetUnitId, AssetUnitDto assetUnit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assetUnit);

        var currentAssetUnit = await _assetUnitReadRepository.GetAssetUnitDtoByIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        if (await _assetUnitWriteRepository.AssetTagExistsAsync(assetUnit.AssetTag, assetUnitId, cancellationToken))
            throw new InvalidOperationException($"Asset tag '{assetUnit.AssetTag}' already exists.");

        if (await _assetUnitWriteRepository.SerialNoExistsAsync(assetUnit.SerialNo, assetUnitId, cancellationToken))
            throw new InvalidOperationException($"Serial no '{assetUnit.SerialNo}' already exists.");

        var entity = await _assetUnitWriteRepository.GetByIdWithHistoriesAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        var fromAvailabilityStatus = entity.AvailabilityStatus;
        var fromOperationalStatus = entity.OperationalStatus;
        var fromResponsibleUserId = entity.ResponsibleUserId;

        entity.Update(
            assetUnit.AssetTag,
            assetUnit.SerialNo,
            assetUnit.Name,
            assetUnit.Brand,
            assetUnit.AvailabilityStatus,
            assetUnit.OperationalStatus,
            assetUnit.Remark,
            assetUnit.ResponsibleUserId);

        entity.AddUpdateHistory(
            Guid.Empty,
            fromAvailabilityStatus,
            entity.AvailabilityStatus,
            fromOperationalStatus,
            entity.OperationalStatus,
            fromResponsibleUserId,
            entity.ResponsibleUserId);

        if (assetUnit.AssetId.HasValue && assetUnit.AssetId.Value != Guid.Empty)
        {
            var asset = await _assetWriteRepository.GetByIdAsync(assetUnit.AssetId.Value, cancellationToken)
                ?? throw new KeyNotFoundException($"Asset with id {assetUnit.AssetId.Value} was not found.");

            entity.AssignAsset(asset);
        }
        else
        {
            entity.UnassignAsset();
        }

        return true;
    }

    public async Task<bool> DeleteAssetUnit(Guid assetUnitId, CancellationToken cancellationToken = default)
    {
        var currentAssetUnit = await _assetUnitReadRepository.GetAssetUnitDtoByIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        await _assetUnitWriteRepository.DeleteAsync(assetUnitId, cancellationToken);

        return true;
    }

    public async Task<List<AssetUnitDto>> GetAssetUnitsByAssetId(Guid assetId, CancellationToken cancellationToken = default)
    {
        if (assetId == Guid.Empty)
            throw new ArgumentException("Asset id is required.", nameof(assetId));

        return await _assetUnitReadRepository.GetAssetUnitsByAssetIdAsync(assetId, cancellationToken);
    }

    public async Task<List<AssetUnitDto>> GetUnassignedAssetUnits(CancellationToken cancellationToken = default)
    {
        return await _assetUnitReadRepository.GetUnassignedAssetUnitsAsync(cancellationToken);
    }

    public async Task<AssetUnitDto> GetAssetUnitById(Guid assetUnitId, CancellationToken cancellationToken = default)
    {
        var assetUnit = await _assetUnitReadRepository.GetAssetUnitDtoByIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        return assetUnit;
    }

    public async Task<AssetUnitDetailDto> GetAssetUnitDetailById(Guid assetUnitId, CancellationToken cancellationToken = default)
    {
        var assetUnit = await _assetUnitReadRepository.GetAssetUnitDetailDtoByIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        return assetUnit;
    }

    public async Task<List<AssetUnitImageDto>> GetAssetUnitImagesByAssetUnitId(
        Guid assetUnitId,
        CancellationToken cancellationToken = default)
    {
        if (assetUnitId == Guid.Empty)
            throw new ArgumentException("Asset unit id is required.", nameof(assetUnitId));

        return await _assetUnitReadRepository.GetAssetUnitImagesByAssetUnitIdAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");
    }

    public async Task<List<AssetUnitImageDto>> AddAssetUnitImages(
        Guid assetUnitId,
        List<CreateAssetUnitImageDto> images,
        CancellationToken cancellationToken = default)
    {
        if (assetUnitId == Guid.Empty)
            throw new ArgumentException("Asset unit id is required.", nameof(assetUnitId));

        ArgumentNullException.ThrowIfNull(images);

        if (images.Count == 0) return [];

        var assetUnit = await _assetUnitWriteRepository.GetByIdWithImagesAsync(assetUnitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset unit with id {assetUnitId} was not found.");

        var addedImages = new List<AssetUnitImageDto>(images.Count);

        foreach (var image in images)
        {
            var addedImage = assetUnit.AddImage(
                image.ImageUrl,
                image.Description,
                image.FileName,
                image.ContentType,
                image.FileSizeBytes);

            addedImages.Add(new AssetUnitImageDto
            {
                Id = addedImage.Id,
                AssetUnitId = assetUnit.Id,
                ImageUrl = addedImage.ImageUrl,
                Description = addedImage.Description,
                FileName = addedImage.FileName,
                ContentType = addedImage.ContentType,
                FileSizeBytes = addedImage.FileSizeBytes
            });
        }

        return addedImages;
    }

}
