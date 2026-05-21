using Shared.Pagination;

namespace Request.Requests.Features.Request.GetRequest;

public record GetRequestQuery(
    PaginationRequest PaginationRequest,
    Guid UserId,
    string RoleCode) : IQuery<GetRequestResult>;
