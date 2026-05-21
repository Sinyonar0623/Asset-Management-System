namespace Asset.Assets.Features.AssetFeature.GetAssetCount;

public record GetAssetCountQuery(Guid UserId, string RoleCode) : IQuery<GetAssetCountResult>;
