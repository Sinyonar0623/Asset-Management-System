using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitDetail;

public class GetAssetUnitDetailQueryHandler(IAssetUnitCommandHandlerService service)
    : IQueryHandler<GetAssetUnitDetailQuery, GetAssetUnitDetailResult>
{
    private readonly IAssetUnitCommandHandlerService _service = service;

    public async Task<GetAssetUnitDetailResult> Handle(
        GetAssetUnitDetailQuery request,
        CancellationToken cancellationToken)
    {
        var assetUnit = await _service.GetAssetUnitDetailById(request.Id, cancellationToken);

        return new GetAssetUnitDetailResult(assetUnit);
    }
}
