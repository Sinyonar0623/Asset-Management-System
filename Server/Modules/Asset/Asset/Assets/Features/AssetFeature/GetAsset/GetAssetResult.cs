using Shared.Pagination;

namespace Asset.Assets.Features.AssetFeature.GetAsset;

public record GetAssetResult(PaginatedResult<AssetDto> Assets);
