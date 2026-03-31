namespace Asset.Assets.Features.AssetFeature.AssignAssetUnit;

public record AssignAssetUnitRequest(Guid AssetId, List<Guid> NewUnits);