using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetFeature.GetAllocatableAsset;

public class GetAllocatableAssetQueryHandler(IAssetCommandHandlerService service)
    : IQueryHandler<GetAllocatableAssetQuery, GetAllocatableAssetResult>
{
    private readonly IAssetCommandHandlerService _service = service;

    public async Task<GetAllocatableAssetResult> Handle(
        GetAllocatableAssetQuery request,
        CancellationToken cancellationToken)
    {
        var assets = await _service.GetAllocatableAssets(
            request.PaginationRequest,
            request.RoleCode,
            cancellationToken);

        return new GetAllocatableAssetResult(assets);
    }
}
