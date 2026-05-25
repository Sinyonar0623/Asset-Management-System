using Shared.Messaging.Integration.Events;

namespace Shared.Messaging.Integration.Command;

public sealed record ReassignHODApproverCommand(
    Guid OldHODApproverId,
    Guid NewHODApproverId
) : IIntegration;
