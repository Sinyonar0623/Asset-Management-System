using Asset.Data.Repository.Read;
using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetFeature.GetAssetHistory;

public class GetAssetHistoryQueryHandler(
    IAssetCommandHandlerService assetService,
    IAssetUnitReadRepository assetUnitReadRepository)
    : IQueryHandler<GetAssetHistoryQuery, GetAssetHistoryResult>
{
    private readonly IAssetCommandHandlerService _assetService = assetService;
    private readonly IAssetUnitReadRepository _assetUnitReadRepository = assetUnitReadRepository;

    public async Task<GetAssetHistoryResult> Handle(
        GetAssetHistoryQuery request,
        CancellationToken cancellationToken)
    {
        await _assetService.GetVisibleAssetById(
            request.AssetId,
            request.UserId,
            request.RoleCode,
            cancellationToken);

        var histories = await _assetUnitReadRepository.GetAssetHistoriesByAssetIdAsync(
            request.AssetId,
            cancellationToken);

        return new GetAssetHistoryResult(histories);
    }
}
