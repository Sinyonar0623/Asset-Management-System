using Asset.Service;
using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitByAssetId;

public class GetAssetUnitByAssetIdQueryHandler(IAssetUnitCommandHandlerService service)
    : IQueryHandler<GetAssetUnitByAssetIdQuery, GetAssetUnitByAssetIdResult>
{
    private readonly IAssetUnitCommandHandlerService _service = service;
    public async Task<GetAssetUnitByAssetIdResult> Handle(GetAssetUnitByAssetIdQuery request, CancellationToken cancellationToken)
    {
        var assetUnits = await _service.GetAssetUnitsByAssetId(request.AssetId, cancellationToken);

        return new GetAssetUnitByAssetIdResult(assetUnits);
    }
}
