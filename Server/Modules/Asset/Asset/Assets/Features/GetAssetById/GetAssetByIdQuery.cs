using Asset.Dto;
using Shared.CQRS;

namespace Asset.Assets.Features.GetAssetById;

public record GetAssetByIdQuery(long Id) : IQuery<GetAssetByIdResult>;

public record GetAssetByIdResult(AssetDto? Asset, bool Found);
