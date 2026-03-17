using Asset.Service;

namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitById;

public class GetAssetUnitByIdQueryHandler(IAssetUnitService service)
    : IQueryHandler<GetAssetUnitByIdQuery, GetAssetUnitByIdResult>
{
    public async Task<GetAssetUnitByIdResult> Handle(GetAssetUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var assetUnit = await service.GetAssetUnit(request.Id, cancellationToken);

        return new GetAssetUnitByIdResult(assetUnit);
    }
}
