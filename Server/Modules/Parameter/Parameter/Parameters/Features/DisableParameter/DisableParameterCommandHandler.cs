namespace Parameter.Parameters.Features.DisableParameter;

public class DisableParameterCommandHandler(IParameterService _service)
    : ICommandHandler<DisableParameterCommand, DisableParameterResult>
{
    public async Task<DisableParameterResult> Handle(DisableParameterCommand request, CancellationToken cancellationToken)
    {
        await _service.DisableParameter(request.Id);

        return new DisableParameterResult(request.Id);
    }
}
