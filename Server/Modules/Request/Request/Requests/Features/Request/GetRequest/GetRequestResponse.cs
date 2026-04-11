using Shared.Pagination;

namespace Request.Requests.Features.Request.GetRequest;

public record GetRequestResponse(PaginatedResult<RequestDto> Requests);
