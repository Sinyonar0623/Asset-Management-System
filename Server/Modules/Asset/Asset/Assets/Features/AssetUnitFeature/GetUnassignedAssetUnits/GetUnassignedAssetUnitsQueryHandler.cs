using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetUnitFeature.GetUnassignedAssetUnits;

public class GetUnassignedAssetUnitsQueryHandler(IAssetUnitCommandHandlerService service)
    : IQueryHandler<GetUnassignedAssetUnitsQuery, GetUnassignedAssetUnitsResult>
{
    private readonly IAssetUnitCommandHandlerService _service = service;

    public async Task<GetUnassignedAssetUnitsResult> Handle(
        GetUnassignedAssetUnitsQuery request,
        CancellationToken cancellationToken)
    {
        var assetUnits = await _service.GetUnassignedAssetUnits(cancellationToken);

        return new GetUnassignedAssetUnitsResult(assetUnits);
    }
}
