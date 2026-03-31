namespace Asset.Assets.Features.AssetFeature.AssignAssetUnit;

public record AssignAssetUnitCommand(Guid AssetId, List<Guid> NewUnits) : ICommand<AssignAssetUnitResult>;