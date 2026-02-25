using Asset.Data.Repository;
using Asset.Dto;
using Shared.CQRS;

namespace Asset.Assets.Features.GetAssets;

public sealed class GetAssetsQueryHandler(IAssetRepository assetRepository)
    : IQueryHandler<GetAssetsQuery, GetAssetsResult>
{
    private readonly IAssetRepository _repository = assetRepository;

    public async Task<GetAssetsResult> Handle(GetAssetsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, totalCount) = await _repository.GetPagedAsync(
            page, pageSize, request.Search, request.Type, request.Status, cancellationToken);

        var summaries = items.Select(a => new AssetSummaryDto(
            a.Id,
            a.RealWorldId,
            a.Brand,
            a.Name,
            a.SerialNo,
            a.Type,
            a.Status,
            a.Amount,
            a.Laboratory?.LaboratoryName,
            a.Laboratory?.RoomNo
        ));

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new GetAssetsResult(new PagedResult<AssetSummaryDto>(summaries, totalCount, page, pageSize, totalPages));
    }
}
