namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitByAssetId;

public record GetAssetUnitByAssetIdQuery(Guid AssetId) : IQuery<GetAssetUnitByAssetIdResult>;
