namespace Parameter.Parameters.Features.EndableParameter;

public class EndableParameterCommandHandler(IParameterService _service)
    : ICommandHandler<EndableParameterCommand, EndableParameterResult>
{
    public async Task<EndableParameterResult> Handle(EndableParameterCommand request, CancellationToken cancellationToken)
    {
        await _service.EnableParameter(request.Id);

        return new EndableParameterResult(request.Id);
    }
}
