namespace Asset.Assets.Features.AssetFeature.CreateAsset;

public record CreateAssetRequest(AssetDto Asset, List<Guid> Units);