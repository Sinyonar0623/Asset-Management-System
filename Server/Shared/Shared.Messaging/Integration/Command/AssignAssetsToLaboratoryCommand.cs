using Shared.Messaging.Integration.Events;

namespace Shared.Messaging.Integration.Command;

public sealed record AssignAssetsToLaboratoryCommand(
    Guid RequestId,
    Guid ApproverId,
    Guid LaboratoryId,
    List<Guid> AssetIds
) : IIntegration;
