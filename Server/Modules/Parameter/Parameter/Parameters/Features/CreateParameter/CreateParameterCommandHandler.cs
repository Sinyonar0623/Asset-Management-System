namespace Parameter.Parameters.Features.CreateParameter;

public class CreateParameterCommandHandler(IParameterService _service)
    : ICommandHandler<CreateParameterCommand, CreateParameterResult>
{
    public async Task<CreateParameterResult> Handle(CreateParameterCommand request, CancellationToken cancellationToken)
    {
        var parameter = await _service.CreateParameter(request.Parameter);

        var result = parameter.Adapt<CreateParameterResult>();

        return result;
    }
}