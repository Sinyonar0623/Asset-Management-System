namespace Parameter.Parameters.Features.DisableParameter;

public class DisableParameterCommandHandler(IParameterService service)
    : ICommandHandler<DisableParameterCommand, DisableParameterResult>
{
    public async Task<DisableParameterResult> Handle(DisableParameterCommand request, CancellationToken cancellationToken)
    {
        await service.DisableParameter(request.Id);

        return new DisableParameterResult(request.Id);
    }
}
