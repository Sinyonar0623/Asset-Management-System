using Shared.Messaging.Integration.Events;

namespace Shared.Messaging.Integration.Command;

public sealed record CreateNewRequestIntegrationCommand(
    Guid ReqId,
    Guid LabId
) : IIntegration;