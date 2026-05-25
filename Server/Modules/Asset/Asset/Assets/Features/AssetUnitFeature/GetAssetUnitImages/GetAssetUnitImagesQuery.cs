namespace Asset.Assets.Features.AssetUnitFeature.GetAssetUnitImages;

public record GetAssetUnitImagesQuery(Guid AssetUnitId) : IQuery<GetAssetUnitImagesResult>;
