using Shared.Pagination;

namespace Asset.Assets.Features.AssetFeature.GetAllocatableAsset;

public record GetAllocatableAssetQuery(
    PaginationRequest PaginationRequest,
    string RoleCode,
    string? SearchTerm) : IQuery<GetAllocatableAssetResult>;
