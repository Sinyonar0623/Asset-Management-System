using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetFeature.GetAssetByLab;

public class GetAssetByLabQueryHandler(IAssetCommandHandlerService service)
    : IQueryHandler<GetAssetByLabQuery, GetAssetByLabResult>
{
    private readonly IAssetCommandHandlerService _service = service;

    public async Task<GetAssetByLabResult> Handle(GetAssetByLabQuery request, CancellationToken cancellationToken)
    {
        var assets = await _service.GetAssetsByLab(request.LaboratoryId, request.PaginationRequest, cancellationToken);
        return new GetAssetByLabResult(assets);
    }
}
