using Shared.Pagination;

namespace Asset.Assets.Features.AssetFeature.GetAllocatableAsset;

public record GetAllocatableAssetResult(PaginatedResult<AssetDto> Assets);
