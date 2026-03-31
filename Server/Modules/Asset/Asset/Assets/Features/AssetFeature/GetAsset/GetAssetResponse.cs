using Shared.Pagination;

namespace Asset.Assets.Features.AssetFeature.GetAsset;

public record GetAssetResponse(PaginatedResult<AssetDto> Assets);
