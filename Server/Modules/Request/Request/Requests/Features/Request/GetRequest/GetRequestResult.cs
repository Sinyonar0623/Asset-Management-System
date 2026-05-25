using Shared.Pagination;

namespace Request.Requests.Features.Request.GetRequest;

public record GetRequestResult(PaginatedResult<RequestDto> Requests);
