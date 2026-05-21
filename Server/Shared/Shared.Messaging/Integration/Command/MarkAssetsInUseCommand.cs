using Shared.Messaging.Integration.Events;

namespace Shared.Messaging.Integration.Command;

public sealed record MarkAssetsInUseCommand(
    Guid RequestId,
    Guid PerformedBy,
    Guid ResponsibleUserId,
    List<Guid> AssetIds
) : IIntegration;
