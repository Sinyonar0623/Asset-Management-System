using Shared.Pagination;

namespace Asset.Assets.Features.AssetFeature.GetAssetByLab;

public record GetAssetByLabResponse(PaginatedResult<AssetDto> Assets);
