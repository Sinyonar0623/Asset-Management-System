using Asset.Dto;
using Shared.CQRS;

namespace Asset.Assets.Features.GetAssets;

public record GetAssetsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? Type,
    string? Status
) : IQuery<GetAssetsResult>;

public record GetAssetsResult(PagedResult<AssetSummaryDto> Data);
