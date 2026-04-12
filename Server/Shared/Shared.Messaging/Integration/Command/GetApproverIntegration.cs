using Shared.Messaging.Integration.Events;

namespace Shared.Messaging.Integration.Command;

public sealed record GetApproverIntegration(
    Guid LabId
) : IIntegration;