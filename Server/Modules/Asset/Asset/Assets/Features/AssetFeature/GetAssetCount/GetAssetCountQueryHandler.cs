using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetFeature.GetAssetCount;

public class GetAssetCountQueryHandler(IAssetCommandHandlerService service)
    : IQueryHandler<GetAssetCountQuery, GetAssetCountResult>
{
    private readonly IAssetCommandHandlerService _service = service;

    public async Task<GetAssetCountResult> Handle(GetAssetCountQuery request, CancellationToken cancellationToken)
    {
        var count = await _service.GetAssetCount(cancellationToken);
        return new GetAssetCountResult(count);
    }
}
