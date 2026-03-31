using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetFeature.GetAsset;

public class GetAssetQueryHandler(IAssetCommandHandlerService service) : IQueryHandler<GetAssetQuery, GetAssetResult>
{
    private readonly IAssetCommandHandlerService _service = service;
    public async Task<GetAssetResult> Handle(GetAssetQuery request, CancellationToken cancellationToken)
    {
        var assets = await _service.GetAssets(request.PaginationRequest, cancellationToken);
        return new GetAssetResult(assets);
    }
}
