namespace Asset.Assets.Features.AssetFeature.GetAssetById;

public record GetAssetByIdQuery(
    Guid Id,
    Guid UserId,
    string RoleCode) : IQuery<GetAssetByIdResult>;
