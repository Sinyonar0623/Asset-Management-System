namespace Asset.Assets.Features.AssetFeature.GetAssetCountByLab;

public record GetAssetCountByLabQuery(Guid LaboratoryId) : IQuery<GetAssetCountByLabResult>;
