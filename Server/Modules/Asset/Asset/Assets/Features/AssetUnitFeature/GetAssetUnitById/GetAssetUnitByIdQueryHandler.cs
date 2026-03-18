using Asset.Service;
using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitById;

public class GetAssetUnitByIdQueryHandler(IAssetUnitCommandHandlerService service)
    : IQueryHandler<GetAssetUnitByIdQuery, GetAssetUnitByIdResult>
{
    private readonly IAssetUnitCommandHandlerService _service = service;
    public async Task<GetAssetUnitByIdResult> Handle(GetAssetUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var assetUnit = await _service.GetAssetUnitById(request.Id, cancellationToken);

        return new GetAssetUnitByIdResult(assetUnit);
    }
}
