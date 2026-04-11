namespace Request.Requests.Events;

public sealed record CreateNewRequestEvent (
    Guid ReqId,
    Guid LabId
) : IDomainEvent;