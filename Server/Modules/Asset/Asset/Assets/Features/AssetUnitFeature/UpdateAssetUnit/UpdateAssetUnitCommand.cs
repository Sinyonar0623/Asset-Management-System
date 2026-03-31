namespace Asset.Assets.Features.AssetUnitFeature.UpdateAssetUnit;

public record UpdateAssetUnitCommand(Guid Id, AssetUnitDto AssetUnit) : ICommand<UpdateAssetUnitResult>;
