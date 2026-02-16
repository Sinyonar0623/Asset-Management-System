using Shared.CQRS;

namespace Asset.Assets.Features.CreateAsset;

public record CreateAssetCommand(
    string RealWorldId,
    string Brand,
    string Name,
    string SerialNo,
    string Description,
    string Type,
    long Amount,
    string Remark,
    Guid OwnerId
) : ICommand<CreateAssetResult>;