namespace Parameter.Parameters.Features.GetParameters;

public class GetParameterQueryHandler(IParameterService _service) : IQueryHandler<GetParameterQuery, GetParameterResult>
{
    public async Task<GetParameterResult> Handle(GetParameterQuery request, CancellationToken cancellationToken)
    {
        var parameters = await _service.GetParameters();
        var pageNumber = Math.Max(0, request.PaginationRequest.PageNumber);
        var pageSize = request.PaginationRequest.PageSize <= 0 ? 10 : request.PaginationRequest.PageSize;

        var items = parameters
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new PaginatedResult<ParameterDto>(items, parameters.Count, pageNumber, pageSize);

        return new GetParameterResult(result);
    }
}
