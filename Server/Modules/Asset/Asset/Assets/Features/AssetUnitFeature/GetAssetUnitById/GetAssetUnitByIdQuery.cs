namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitById;

public record GetAssetUnitByIdQuery(Guid Id) : IQuery<GetAssetUnitByIdResult>;
