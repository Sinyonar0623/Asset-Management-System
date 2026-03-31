namespace Asset.Assets.Features.AssetUnitFeature.DeleteAssetUnit;

public record DeleteAssetUnitCommand(Guid Id) : ICommand<DeleteAssetUnitResult>;
