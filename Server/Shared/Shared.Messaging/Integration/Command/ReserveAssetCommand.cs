using Shared.Messaging.Integration.Events;

namespace Shared.Messaging.Integration.Command;

public sealed record ReserveAssetCommand(
    Guid RequestId,
    Guid ApproverId,
    List<Guid> AssetIds
) : IIntegration;
