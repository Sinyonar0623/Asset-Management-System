using Asset.Assets.Events;
using Asset.Data.Repository.Read;
using Asset.Data.Repository.Write;
using Microsoft.EntityFrameworkCore;
using Shared.Data.UnitOfWork;
using Shared.Pagination;

namespace Asset.Service.CommandHandlerService;

public class AssetCommandHandlerService(
    IAssetWriteRepository assetWriteRepository,
    IAssetReadRepository assetReadRepository
    ) : IAssetCommandHandlerService
{
    private readonly IAssetWriteRepository _assetWriteRepository = assetWriteRepository;
    private readonly IAssetReadRepository _assetReadRepository = assetReadRepository;

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

    public async Task<bool> ReserveAssetsAsync(List<Guid> assetIds, CancellationToken cancellationToken)
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
            var isReserved = await _assetWriteRepository.TryReserveAsync(assetId, cancellationToken);
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

    public async Task<bool> ReleaseAssetsAsync(List<Guid> assetIds, CancellationToken cancellationToken)
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
            var isReleased = await _assetWriteRepository.TryReleaseAsync(assetId, cancellationToken);
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
        var asset = await _assetReadRepository.GetByIdAsync(assetId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset with id {assetId} was not found.");

        return new AssetDto
        {
            Id = asset.Id,
            Name = asset.Name,
            Description = asset.Description,
            Category = asset.Category,
            IsAvailable = asset.IsAvailable
        };
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

        return await _assetReadRepository.GetPaginatedAsync(
            request,
            x => new AssetDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Category = x.Category,
                IsAvailable = x.IsAvailable
            },
            cancellationToken);
    }

    public async Task<PaginatedResult<AssetDto>> GetAssetsByLab(
        Guid laboratoryId,
        PaginationRequest paginationRequest,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = Math.Max(0, paginationRequest.PageNumber);
        var pageSize = paginationRequest.PageSize <= 0 ? 10 : paginationRequest.PageSize;
        var request = new PaginationRequest(pageNumber, pageSize);

        return await _assetReadRepository.GetPaginatedAsync(
            request,
            x => EF.Property<Guid?>(x, "LaboratoryId") == laboratoryId,
            x => new AssetDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Category = x.Category,
                IsAvailable = x.IsAvailable
            },
            cancellationToken);
    }

    public Task<long> GetAssetCount(CancellationToken cancellationToken = default)
    {
        return _assetReadRepository.GetAssetCountAsync(cancellationToken);
    }

    public Task<long> GetAssetCountByLab(Guid laboratoryId, CancellationToken cancellationToken = default)
    {
        return _assetReadRepository.GetAssetCountByLaboratoryIdAsync(laboratoryId, cancellationToken);
    }
}
