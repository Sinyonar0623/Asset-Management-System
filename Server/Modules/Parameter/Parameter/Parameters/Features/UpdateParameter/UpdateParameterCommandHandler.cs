namespace Parameter.Parameters.Features.UpdateParameter;

public class UpdateParameterCommandHandler(IParameterService service)
    : ICommandHandler<UpdateParameterCommand, UpdateParameterResult>
{
    public async Task<UpdateParameterResult> Handle(UpdateParameterCommand request, CancellationToken cancellationToken)
    {
        await service.UpdateParameter(request.Id, request.Parameter);

        return new UpdateParameterResult(request.Id);
    }
}
