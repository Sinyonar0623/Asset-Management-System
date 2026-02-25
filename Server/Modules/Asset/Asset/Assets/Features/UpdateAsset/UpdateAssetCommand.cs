using Shared.CQRS;

namespace Asset.Assets.Features.UpdateAsset;

public record UpdateAssetCommand(
    long Id,
    string Brand,
    string Name,
    string Description,
    string Type,
    string Status,
    long Amount,
    string Remark
) : ICommand<UpdateAssetResult>;

public record UpdateAssetResult(bool Succeeded, string? Error);

public record UpdateAssetRequest(
    string Brand,
    string Name,
    string Description,
    string Type,
    string Status,
    long Amount,
    string Remark
);
