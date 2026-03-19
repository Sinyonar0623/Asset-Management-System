using Shared.Pagination;

namespace Asset.Assets.Features.AssetFeature.GetAssetByLab;

public record GetAssetByLabQuery(Guid LaboratoryId, PaginationRequest PaginationRequest) : IQuery<GetAssetByLabResult>;
