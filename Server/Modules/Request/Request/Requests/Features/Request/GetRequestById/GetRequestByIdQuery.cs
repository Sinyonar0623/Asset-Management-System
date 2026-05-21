namespace Request.Requests.Features.Request.GetRequestById;

public record GetRequestByIdQuery(
    Guid Id,
    Guid UserId,
    string RoleCode) : IQuery<GetRequestByIdResult>;
