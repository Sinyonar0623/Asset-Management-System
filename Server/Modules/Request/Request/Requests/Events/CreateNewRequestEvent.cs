namespace Request.Requests.Events;

public sealed record CreateNewRequestEvent (
    Guid ReqId,
    Guid LabId,
    string RequestType,
    Guid RequesterId,
    string RequesterRoleCode
) : IDomainEvent;
