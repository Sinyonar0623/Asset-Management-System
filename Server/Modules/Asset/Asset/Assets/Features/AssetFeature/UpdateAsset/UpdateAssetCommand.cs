namespace Asset.Assets.Features.AssetFeature.UpdateAsset;

public record UpdateAssetCommand(Guid Id, AssetDto Asset) : ICommand<UpdateAssetResult>;
