using Asset.Service;

namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitByAssetId;

public class GetAssetUnitByAssetIdQueryHandler(IAssetUnitService service)
    : IQueryHandler<GetAssetUnitByAssetIdQuery, GetAssetUnitByAssetIdResult>
{
    public async Task<GetAssetUnitByAssetIdResult> Handle(GetAssetUnitByAssetIdQuery request, CancellationToken cancellationToken)
    {
        var assetUnits = await service.GetAssetUnitsByAssetId(request.AssetId, cancellationToken);

        return new GetAssetUnitByAssetIdResult(assetUnits);
    }
}
