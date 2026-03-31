namespace Asset.Assets.Features.AssetFeature.GetAssetById;

public record GetAssetByIdQuery(Guid Id) : IQuery<GetAssetByIdResult>;
