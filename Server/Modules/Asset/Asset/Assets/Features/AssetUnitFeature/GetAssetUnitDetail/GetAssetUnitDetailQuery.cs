namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitDetail;

public record GetAssetUnitDetailQuery(Guid Id) : IQuery<GetAssetUnitDetailResult>;
