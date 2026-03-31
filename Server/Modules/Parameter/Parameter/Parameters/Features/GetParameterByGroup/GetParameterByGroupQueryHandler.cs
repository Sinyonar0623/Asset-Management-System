namespace Parameter.Parameters.Features.GetParameterByGroup;

public class GetParameterByGroupQueryHandler(IParameterService _service)
    : IQueryHandler<GetParameterByGroupQuery, GetParameterByGroupResult>
{
    public async Task<GetParameterByGroupResult> Handle(GetParameterByGroupQuery request, CancellationToken cancellationToken)
    {
        var parameters = await _service.GetParameterByGroup(request.Group);
        var pageNumber = Math.Max(0, request.PaginationRequest.PageNumber);
        var pageSize = request.PaginationRequest.PageSize <= 0 ? 10 : request.PaginationRequest.PageSize;

        var result = parameters
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToList();

        return new GetParameterByGroupResult(result);
    }
}
