using Shared.Pagination;

namespace Asset.Assets.Features.AssetFeature.GetAssetByLab;

public record GetAssetByLabResult(PaginatedResult<AssetDto> Assets);
