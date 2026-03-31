using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetFeature.GetAssetCountByLab;

public class GetAssetCountByLabQueryHandler(IAssetCommandHandlerService service)
    : IQueryHandler<GetAssetCountByLabQuery, GetAssetCountByLabResult>
{
    private readonly IAssetCommandHandlerService _service = service;

    public async Task<GetAssetCountByLabResult> Handle(GetAssetCountByLabQuery request, CancellationToken cancellationToken)
    {
        var count = await _service.GetAssetCountByLab(request.LaboratoryId, cancellationToken);
        return new GetAssetCountByLabResult(count);
    }
}
