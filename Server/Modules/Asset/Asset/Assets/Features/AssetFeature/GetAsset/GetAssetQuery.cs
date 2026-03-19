using Shared.Pagination;

namespace Asset.Assets.Features.AssetFeature.GetAsset;

public record GetAssetQuery(PaginationRequest PaginationRequest) : IQuery<GetAssetResult>;
