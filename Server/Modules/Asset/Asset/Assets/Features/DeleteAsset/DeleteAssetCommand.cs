using Shared.CQRS;

namespace Asset.Assets.Features.DeleteAsset;

public record DeleteAssetCommand(long Id) : ICommand<DeleteAssetResult>;

public record DeleteAssetResult(bool Succeeded, string? Error);
