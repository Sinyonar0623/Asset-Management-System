using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetFeature.GetAssetById;

public class GetAssetByIdQueryHandler(IAssetCommandHandlerService service)
    : IQueryHandler<GetAssetByIdQuery, GetAssetByIdResult>
{
    private readonly IAssetCommandHandlerService _service = service;

    public async Task<GetAssetByIdResult> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
    {
        var asset = await _service.GetAssetById(request.Id, cancellationToken);
        return new GetAssetByIdResult(asset);
    }
}
