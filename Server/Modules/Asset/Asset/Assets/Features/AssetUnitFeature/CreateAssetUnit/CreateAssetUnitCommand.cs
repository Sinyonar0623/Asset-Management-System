namespace Asset.Assets.Features.AssetUnitFeature.CreateAssetUnit;

public record CreateAssetUnitCommand(List<AssetUnitDto> AssetUnits) : ICommand<CreateAssetUnitResult>;
