namespace Asset.Assets.Features.AssetFeature.GetAssetHistory;

public record GetAssetHistoryQuery(
    Guid AssetId,
    Guid UserId,
    string RoleCode) : IQuery<GetAssetHistoryResult>;
