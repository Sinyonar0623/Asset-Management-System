namespace Parameter.Parameters.Features.EndableParameter;

public class EndableParameterCommandHandler(IParameterService service)
    : ICommandHandler<EndableParameterCommand, EndableParameterResult>
{
    public async Task<EndableParameterResult> Handle(EndableParameterCommand request, CancellationToken cancellationToken)
    {
        await service.EnableParameter(request.Id);

        return new EndableParameterResult(request.Id);
    }
}
