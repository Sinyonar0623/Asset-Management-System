using Shared.Pagination;

namespace Request.Requests.Features.Request.GetRequest;

public record GetRequestQuery(PaginationRequest PaginationRequest) : IQuery<GetRequestResult>;
