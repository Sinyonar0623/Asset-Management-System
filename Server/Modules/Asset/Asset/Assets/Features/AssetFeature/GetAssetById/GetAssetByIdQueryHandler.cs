using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetFeature.GetAssetById;

public class GetAssetByIdQueryHandler(IAssetCommandHandlerService service)
    : IQueryHandler<GetAssetByIdQuery, GetAssetByIdResult>
{
    private readonly IAssetCommandHandlerService _service = service;

    public async Task<GetAssetByIdResult> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
    {
        var asset = await _service.GetVisibleAssetById(
            request.Id,
            request.UserId,
            request.RoleCode,
            cancellationToken);

        return new GetAssetByIdResult(asset);
    }
}
