namespace Asset.Assets.Features.AssetFeature.ReturnAsset;

public sealed record ReturnAssetCommand(
    Guid AssetId,
    Guid UserId,
    string RoleCode
) : ICommand<ReturnAssetResult>;
