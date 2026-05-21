using Shared.Pagination;

namespace Asset.Assets.Features.AssetFeature.GetAssetByLab;

public record GetAssetByLabQuery(
    Guid LaboratoryId,
    PaginationRequest PaginationRequest,
    Guid UserId,
    string RoleCode) : IQuery<GetAssetByLabResult>;
