using Shared.Pagination;

namespace Asset.Assets.Features.AssetFeature.GetAllocatableAsset;

public record GetAllocatableAssetResponse(PaginatedResult<AssetDto> Assets);
