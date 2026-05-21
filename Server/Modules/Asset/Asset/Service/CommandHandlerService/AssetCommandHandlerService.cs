using Asset.Assets.Events;
using Asset.Data.Repository.Read;
using Asset.Data.Repository.Write;
using Microsoft.EntityFrameworkCore;
using Shared.Data.UnitOfWork;
using Shared.Pagination;
using Shared.Security;
using System.Linq.Expressions;

namespace Asset.Service.CommandHandlerService;

public class AssetCommandHandlerService(
    IAssetWriteRepository assetWriteRepository,
    IAssetReadRepository assetReadRepository,
    ILaboratoryWriteRepository laboratoryWriteRepository
    ) : IAssetCommandHandlerService
{
    private const string DefaultLocation = "G-600";

    private readonly IAssetWriteRepository _assetWriteRepository = assetWriteRepository;
    private readonly IAssetReadRepository _assetReadRepository = assetReadRepository;
    private readonly ILaboratoryWriteRepository _laboratoryWriteRepository = laboratoryWriteRepository;

    private static Expression<Func<Assets.Model.Asset, AssetDto>> AssetDtoProjection => x => new AssetDto
    {
        Id = x.Id,
        Name = x.Name,
        Description = x.Description,
        Category = x.Category,
        IsAvailable = x.IsAvailable,
        AvailabilityStatus = x.IsAvailable ? Assets.Model.AssetUnitStatuses.Availability.Available : Assets.Model.AssetUnitStatuses.Availability.Unavailable,
        Location = x.Laboratory != null ? x.Laboratory.RoomNo : DefaultLocation,
        UpdatedAt = x.UpdateOn ?? x.CreateOn
    };

    public Task<bool> AssignAssetUnits(Assets.Model.Asset asset, List<Guid> units)
    {
        if (asset is null) throw new KeyNotFoundException($"Asset was not found.");

        var _event = new AssignAssetUnitsEvent(asset, units);

        asset.AddDomainEvent(_event);

        return Task.FromResult(true);
    }

    public async Task<bool> AssignAssetUnitsAsync(Guid assetId, List<Guid> units, CancellationToken cancellationToken)
    {
        var asset = await _assetWriteRepository.GetByIdAsync(assetId, cancellationToken) ?? throw new KeyNotFoundException($"Asset was not found.");

        var _event = new AssignAssetUnitsEvent(asset, units);

        asset.AddDomainEvent(_event);

        return true;
    }

    public async Task<bool> ReserveAssetsAsync(
        List<Guid> assetIds,
        Guid performedBy,
        Guid? requestId,
        CancellationToken cancellationToken)
    {
        if (assetIds is null || assetIds.Count == 0)
        {
            throw new ArgumentException("At least one asset id is required.", nameof(assetIds));
        }

        var distinctAssetIds = assetIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (distinctAssetIds.Count == 0)
        {
            throw new ArgumentException("At least one valid asset id is required.", nameof(assetIds));
        }

        foreach (var assetId in distinctAssetIds)
        {
            var isReserved = await _assetWriteRepository.TryReserveAsync(
                assetId,
                performedBy,
                requestId,
                cancellationToken);
            if (isReserved)
            {
                continue;
            }

            var asset = await _assetWriteRepository.GetByIdAsync(assetId, cancellationToken);
            if (asset is null)
            {
                throw new KeyNotFoundException($"Asset with id {assetId} was not found.");
            }

            throw new InvalidOperationException($"Asset with id {assetId} is not available for reservation.");
        }

        return true;
    }

    public async Task<bool> MarkAssetsInUseAsync(
        List<Guid> assetIds,
        Guid responsibleUserId,
        Guid performedBy,
        Guid? requestId,
        CancellationToken cancellationToken)
    {
        if (responsibleUserId == Guid.Empty)
        {
            throw new ArgumentException("Responsible user id is required.", nameof(responsibleUserId));
        }

        var distinctAssetIds = assetIds?
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList() ?? [];

        if (distinctAssetIds.Count == 0)
        {
            throw new ArgumentException("At least one valid asset id is required.", nameof(assetIds));
        }

        foreach (var assetId in distinctAssetIds)
        {
            var isMarkedInUse = await _assetWriteRepository.TryMarkInUseAsync(
                assetId,
                responsibleUserId,
                performedBy,
                requestId,
                cancellationToken);

            if (isMarkedInUse)
            {
                continue;
            }

            var asset = await _assetWriteRepository.GetByIdAsync(assetId, cancellationToken);
            if (asset is null)
            {
                throw new KeyNotFoundException($"Asset with id {assetId} was not found.");
            }

            throw new InvalidOperationException($"Asset with id {assetId} could not be marked as in use.");
        }

        return true;
    }

    public async Task<bool> ReleaseAssetsAsync(
        List<Guid> assetIds,
        Guid performedBy,
        Guid? requestId,
        string? remark,
        CancellationToken cancellationToken)
    {
        if (assetIds is null || assetIds.Count == 0)
        {
            return true;
        }

        var distinctAssetIds = assetIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        foreach (var assetId in distinctAssetIds)
        {
            var isReleased = await _assetWriteRepository.TryReleaseAsync(
                assetId,
                performedBy,
                requestId,
                remark,
                cancellationToken: cancellationToken);
            if (isReleased)
            {
                continue;
            }

            var asset = await _assetWriteRepository.GetByIdAsync(assetId, cancellationToken);
            if (asset is null)
            {
                throw new KeyNotFoundException($"Asset with id {assetId} was not found.");
            }
        }

        return true;
    }

    public async Task<bool> ReturnVisibleAssetAsync(
        Guid assetId,
        Guid userId,
        string roleCode,
        CancellationToken cancellationToken)
    {
        if (assetId == Guid.Empty)
        {
            throw new ArgumentException("Asset id is required.", nameof(assetId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id is required.", nameof(userId));
        }

        var normalizedRoleCode = NormalizeRoleCode(roleCode);
        if (normalizedRoleCode is not (RoleCodes.Student or RoleCodes.Teacher or RoleCodes.Hod or RoleCodes.Admin))
        {
            throw new UnauthorizedAccessException("Your role is not allowed to return assets.");
        }

        await GetVisibleAssetById(assetId, userId, normalizedRoleCode, cancellationToken);

        return await ReleaseAssetsAsync(
            [assetId],
            userId,
            null,
            "Returned by user.",
            cancellationToken);
    }

    public async Task<bool> AssignAssetsToLaboratoryAsync(
        Guid laboratoryId,
        List<Guid> assetIds,
        Guid performedBy,
        Guid? requestId,
        CancellationToken cancellationToken)
    {
        if (laboratoryId == Guid.Empty)
        {
            throw new ArgumentException("Laboratory id is required.", nameof(laboratoryId));
        }

        var distinctAssetIds = assetIds?
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList() ?? [];

        if (distinctAssetIds.Count == 0)
        {
            throw new ArgumentException("At least one asset id is required.", nameof(assetIds));
        }

        var allocatableAssetCount = await _assetReadRepository.CountAsync(
            x => distinctAssetIds.Contains(x.Id)
                 && EF.Property<Guid?>(x, "LaboratoryId") == null,
            cancellationToken);

        if (allocatableAssetCount != distinctAssetIds.Count)
        {
            throw new InvalidOperationException("All allocated assets must be in central storage.");
        }

        var laboratory = await _laboratoryWriteRepository.GetByIdAsync(laboratoryId, cancellationToken);

        if (laboratory is null)
        {
            throw new KeyNotFoundException($"Laboratory with id {laboratoryId} was not found.");
        }

        foreach (var assetId in distinctAssetIds)
        {
            var asset = await _assetWriteRepository.GetByIdAsync(assetId, cancellationToken)
                ?? throw new KeyNotFoundException($"Asset with id {assetId} was not found.");

            asset.AssignLaboratory(laboratory);
            await _assetWriteRepository.TryReleaseAsync(
                assetId,
                performedBy,
                requestId,
                "Allocated to laboratory and made available.",
                "ALLOCATE_TO_LAB",
                cancellationToken);
        }

        return true;
    }

    public async Task<Guid> CreateAsset(AssetDto asset, List<Guid> units, CancellationToken cancellationToken)
    {
        var newAsset = Assets.Model.Asset.Create(
            asset.Name,
            asset.Description,
            asset.Category
        );

        await _assetWriteRepository.AddAsync(newAsset, cancellationToken);

        if (units is not null) await AssignAssetUnits(newAsset, units);

        return newAsset.Id;
    }

    public async Task<AssetDto> GetAssetById(Guid assetId, CancellationToken cancellationToken)
    {
        var asset = await _assetReadRepository.GetByIdAsync(
            assetId,
            AssetDtoProjection,
            cancellationToken)
            ?? throw new KeyNotFoundException($"Asset with id {assetId} was not found.");

        return await EnrichAssetStatus(asset, cancellationToken);
    }

    public async Task<AssetDto> GetVisibleAssetById(
        Guid assetId,
        Guid userId,
        string roleCode,
        CancellationToken cancellationToken)
    {
        var normalizedRoleCode = NormalizeRoleCode(roleCode);

        if (normalizedRoleCode is RoleCodes.Admin or RoleCodes.Hod)
        {
            return await GetAssetById(assetId, cancellationToken);
        }

        AssetDto? asset = null;

        if (normalizedRoleCode is RoleCodes.Teacher)
        {
            asset = await _assetReadRepository.FirstOrDefaultAsync(
                x => x.Id == assetId
                     && x.Laboratory != null
                     && x.Laboratory.TeacherId == userId,
                AssetDtoProjection,
                cancellationToken);
        }
        else if (normalizedRoleCode is RoleCodes.Student)
        {
            var visibleAssetIds = await _assetReadRepository.GetRequestedAssetIdsByRequesterAsync(userId, cancellationToken);
            if (visibleAssetIds.Contains(assetId))
            {
                asset = await _assetReadRepository.GetByIdAsync(assetId, AssetDtoProjection, cancellationToken);
            }
        }

        return await EnrichAssetStatus(
            asset ?? throw new KeyNotFoundException($"Asset with id {assetId} was not found."),
            cancellationToken);
    }

    public async Task<bool> UpdateAsset(Guid assetId, AssetDto asset, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(asset);

        var currentAsset = await _assetWriteRepository.GetByIdAsync(assetId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset with id {assetId} was not found.");

        currentAsset.Update(asset.Name, asset.Description, asset.Category);

        return true;
    }

    public async Task<bool> DeleteAsset(Guid assetId, CancellationToken cancellationToken)
    {
        var currentAsset = await _assetWriteRepository.GetByIdAsync(assetId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset with id {assetId} was not found.");

        await _assetWriteRepository.DeleteAsync(currentAsset, cancellationToken);

        return true;
    }

    public async Task<PaginatedResult<AssetDto>> GetAssets(
        PaginationRequest paginationRequest,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = Math.Max(0, paginationRequest.PageNumber);
        var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
        var request = new PaginationRequest(pageNumber, pageSize);

        var assets = await _assetReadRepository.GetPaginatedAsync(
            request,
            AssetDtoProjection,
            cancellationToken);

        return await EnrichAssetStatuses(assets, cancellationToken);
    }

    public async Task<PaginatedResult<AssetDto>> GetVisibleAssets(
        PaginationRequest paginationRequest,
        Guid userId,
        string roleCode,
        CancellationToken cancellationToken = default)
    {
        var request = NormalizePaginationRequest(paginationRequest);
        var normalizedRoleCode = NormalizeRoleCode(roleCode);

        if (normalizedRoleCode is RoleCodes.Admin or RoleCodes.Hod)
        {
            return await GetAssets(request, cancellationToken);
        }

        if (normalizedRoleCode is RoleCodes.Teacher)
        {
            var assets = await _assetReadRepository.GetPaginatedAsync(
                request,
                x => x.Laboratory != null && x.Laboratory.TeacherId == userId,
                AssetDtoProjection,
                cancellationToken);

            return await EnrichAssetStatuses(assets, cancellationToken);
        }

        if (normalizedRoleCode is RoleCodes.Student)
        {
            var visibleAssetIds = await _assetReadRepository.GetRequestedAssetIdsByRequesterAsync(userId, cancellationToken);
            if (visibleAssetIds.Count == 0)
            {
                return EmptyAssets(request);
            }

            var assets = await _assetReadRepository.GetPaginatedAsync(
                request,
                x => visibleAssetIds.Contains(x.Id),
                AssetDtoProjection,
                cancellationToken);

            return await EnrichAssetStatuses(assets, cancellationToken);
        }

        return EmptyAssets(request);
    }

    public async Task<PaginatedResult<AssetDto>> GetAssetsByLab(
        Guid laboratoryId,
        PaginationRequest paginationRequest,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = Math.Max(0, paginationRequest.PageNumber);
        var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
        var request = new PaginationRequest(pageNumber, pageSize);

        var assets = await _assetReadRepository.GetPaginatedAsync(
            request,
            x => EF.Property<Guid?>(x, "LaboratoryId") == laboratoryId,
            AssetDtoProjection,
            cancellationToken);

        return await EnrichAssetStatuses(assets, cancellationToken);
    }

    public async Task<PaginatedResult<AssetDto>> GetVisibleAssetsByLab(
        Guid laboratoryId,
        PaginationRequest paginationRequest,
        Guid userId,
        string roleCode,
        CancellationToken cancellationToken = default)
    {
        var request = NormalizePaginationRequest(paginationRequest);
        var normalizedRoleCode = NormalizeRoleCode(roleCode);

        if (normalizedRoleCode is RoleCodes.Admin or RoleCodes.Hod)
        {
            return await GetAssetsByLab(laboratoryId, request, cancellationToken);
        }

        if (normalizedRoleCode is RoleCodes.Teacher)
        {
            var assets = await _assetReadRepository.GetPaginatedAsync(
                request,
                x => EF.Property<Guid?>(x, "LaboratoryId") == laboratoryId
                     && x.Laboratory != null
                     && x.Laboratory.TeacherId == userId,
                AssetDtoProjection,
                cancellationToken);

            return await EnrichAssetStatuses(assets, cancellationToken);
        }

        if (normalizedRoleCode is RoleCodes.Student)
        {
            var visibleAssetIds = await _assetReadRepository.GetRequestedAssetIdsByRequesterAsync(userId, cancellationToken);
            if (visibleAssetIds.Count == 0)
            {
                return EmptyAssets(request);
            }

            var assets = await _assetReadRepository.GetPaginatedAsync(
                request,
                x => EF.Property<Guid?>(x, "LaboratoryId") == laboratoryId
                     && visibleAssetIds.Contains(x.Id),
                AssetDtoProjection,
                cancellationToken);

            return await EnrichAssetStatuses(assets, cancellationToken);
        }

        return EmptyAssets(request);
    }

    public async Task<PaginatedResult<AssetDto>> GetAllocatableAssets(
        PaginationRequest paginationRequest,
        string roleCode,
        CancellationToken cancellationToken = default)
    {
        var request = NormalizePaginationRequest(paginationRequest);
        var normalizedRoleCode = NormalizeRoleCode(roleCode);

        if (normalizedRoleCode is not (RoleCodes.Admin or RoleCodes.Hod or RoleCodes.Teacher))
        {
            return EmptyAssets(request);
        }

        var assets = await _assetReadRepository.GetPaginatedAsync(
            request,
            x => EF.Property<Guid?>(x, "LaboratoryId") == null,
            AssetDtoProjection,
            cancellationToken);

        return await EnrichAssetStatuses(assets, cancellationToken);
    }

    public Task<long> GetAssetCount(CancellationToken cancellationToken = default)
    {
        return _assetReadRepository.GetAssetCountAsync(cancellationToken);
    }

    public async Task<long> GetVisibleAssetCount(
        Guid userId,
        string roleCode,
        CancellationToken cancellationToken = default)
    {
        var normalizedRoleCode = NormalizeRoleCode(roleCode);

        if (normalizedRoleCode is RoleCodes.Admin or RoleCodes.Hod)
        {
            return await GetAssetCount(cancellationToken);
        }

        if (normalizedRoleCode is RoleCodes.Teacher)
        {
            return await _assetReadRepository.CountAsync(
                x => x.Laboratory != null && x.Laboratory.TeacherId == userId,
                cancellationToken);
        }

        if (normalizedRoleCode is RoleCodes.Student)
        {
            var visibleAssetIds = await _assetReadRepository.GetRequestedAssetIdsByRequesterAsync(userId, cancellationToken);
            return visibleAssetIds.Count == 0
                ? 0
                : await _assetReadRepository.CountAsync(x => visibleAssetIds.Contains(x.Id), cancellationToken);
        }

        return 0;
    }

    public Task<long> GetAssetCountByLab(Guid laboratoryId, CancellationToken cancellationToken = default)
    {
        return _assetReadRepository.GetAssetCountByLaboratoryIdAsync(laboratoryId, cancellationToken);
    }

    private static PaginationRequest NormalizePaginationRequest(PaginationRequest paginationRequest)
    {
        var pageNumber = Math.Max(0, paginationRequest.PageNumber);
        var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;

        return new PaginationRequest(pageNumber, pageSize);
    }

    private static PaginatedResult<AssetDto> EmptyAssets(PaginationRequest request)
    {
        return new PaginatedResult<AssetDto>([], 0, request.PageNumber, request.PageSize);
    }

    private static string NormalizeRoleCode(string roleCode)
    {
        return string.IsNullOrWhiteSpace(roleCode)
            ? string.Empty
            : roleCode.Trim().ToUpperInvariant();
    }

    private async Task<AssetDto> EnrichAssetStatus(
        AssetDto asset,
        CancellationToken cancellationToken)
    {
        if (!asset.Id.HasValue)
        {
            return asset;
        }

        var statuses = await _assetReadRepository.GetAvailabilityStatusesByAssetIdsAsync(
            [asset.Id.Value],
            cancellationToken);

        return statuses.TryGetValue(asset.Id.Value, out var status)
            ? ApplyAvailabilityStatus(asset, status)
            : asset;
    }

    private async Task<PaginatedResult<AssetDto>> EnrichAssetStatuses(
        PaginatedResult<AssetDto> assets,
        CancellationToken cancellationToken)
    {
        var items = assets.Items.ToList();
        var assetIds = items
            .Where(x => x.Id.HasValue)
            .Select(x => x.Id!.Value)
            .ToList();
        var statuses = await _assetReadRepository.GetAvailabilityStatusesByAssetIdsAsync(
            assetIds,
            cancellationToken);
        var enrichedItems = items
            .Select(x => x.Id.HasValue && statuses.TryGetValue(x.Id.Value, out var status)
                ? ApplyAvailabilityStatus(x, status)
                : x)
            .ToList();

        return new PaginatedResult<AssetDto>(
            enrichedItems,
            assets.Count,
            assets.PageNumber,
            assets.PageSize);
    }

    private static AssetDto ApplyAvailabilityStatus(AssetDto asset, string status)
    {
        var normalizedStatus = status == Assets.Model.AssetUnitStatuses.Availability.Available
                               && asset.IsAvailable == false
            ? Assets.Model.AssetUnitStatuses.Availability.Unavailable
            : status;

        return asset with
        {
            AvailabilityStatus = normalizedStatus,
            IsAvailable = normalizedStatus == Assets.Model.AssetUnitStatuses.Availability.Available
        };
    }
}
