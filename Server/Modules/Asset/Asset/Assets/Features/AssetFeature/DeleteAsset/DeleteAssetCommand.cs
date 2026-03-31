namespace Asset.Assets.Features.AssetFeature.DeleteAsset;

public record DeleteAssetCommand(Guid Id) : ICommand<DeleteAssetResult>;
