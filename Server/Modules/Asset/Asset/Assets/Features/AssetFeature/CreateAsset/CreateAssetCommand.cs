namespace Asset.Assets.Features.AssetFeature.CreateAsset;

public record CreateAssetCommand(AssetDto Asset, List<Guid> Units) : ICommand<CreateAssetResult>;