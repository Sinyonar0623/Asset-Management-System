namespace Parameter.Parameters.Features.GetParameterById;

public class GetParameterByIdQueryHandler(IParameterService _service)
    : IQueryHandler<GetParameterByIdQuery, GetParameterByIdResult>
{
    public async Task<GetParameterByIdResult> Handle(GetParameterByIdQuery request, CancellationToken cancellationToken)
    {
        var parameter = await _service.GetParameter(request.Id);

        return new GetParameterByIdResult(parameter); 
    }
}