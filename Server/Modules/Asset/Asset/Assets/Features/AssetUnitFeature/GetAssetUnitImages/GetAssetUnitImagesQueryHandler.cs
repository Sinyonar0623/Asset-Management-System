using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitImages;

public class GetAssetUnitImagesQueryHandler(IAssetUnitCommandHandlerService service)
    : IQueryHandler<GetAssetUnitImagesQuery, GetAssetUnitImagesResult>
{
    private readonly IAssetUnitCommandHandlerService _service = service;

    public async Task<GetAssetUnitImagesResult> Handle(
        GetAssetUnitImagesQuery request,
        CancellationToken cancellationToken)
    {
        var images = await _service.GetAssetUnitImagesByAssetUnitId(
            request.AssetUnitId,
            cancellationToken);

        return new GetAssetUnitImagesResult(images);
    }
}
