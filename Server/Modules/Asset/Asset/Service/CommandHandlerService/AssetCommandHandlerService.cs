using Asset.Assets.Events;
using Asset.Data.Repository.Read;
using Asset.Data.Repository.Write;
using Shared.Data.UnitOfWork;

namespace Asset.Service.CommandHandlerService;

public class AssetCommandHandlerService(
    IAssetWriteRepository assetWriteRepository,
    IAssetReadRepository assetReadRepository
    ) : IAssetCommandHandlerService
{
    private readonly IAssetWriteRepository _assetWriteRepository = assetWriteRepository;
    private readonly IAssetReadRepository _assetReadRepository = assetReadRepository;
    public async Task AssignAssetUnits(Guid assetId, Assets.Model.Asset asset, List<Guid> units, CancellationToken cancellationToken)
    {
        if (asset is null)
        {
            var entity = await _assetWriteRepository.GetByIdAsync(assetId, cancellationToken)
                ?? throw new KeyNotFoundException($"Asset with id {assetId} was not found.");

            asset = entity;
        }

        var _event = new AssignAssetUnitsEvent(asset, units);

        asset.AddDomainEvent(_event);
    }

    public async Task<Guid> CreateAsset(AssetDto asset, List<Guid> units, CancellationToken cancellationToken)
    {
        var newAsset = Assets.Model.Asset.Create(
            asset.Name,
            asset.Description,
            asset.Category
        );

        await _assetWriteRepository.AddAsync(newAsset, cancellationToken);

        if (units is not null) await AssignAssetUnits(newAsset.Id, newAsset, units, cancellationToken);

        return newAsset.Id;
    }
}