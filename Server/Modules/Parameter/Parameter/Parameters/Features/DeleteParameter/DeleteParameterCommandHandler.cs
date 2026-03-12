namespace Parameter.Parameters.Features.DeleteParameter;

public class DeleteParameterCommandHandler(IParameterService service)
    : ICommandHandler<DeleteParameterCommand, DeleteParameterResult>
{
    public async Task<DeleteParameterResult> Handle(DeleteParameterCommand request, CancellationToken cancellationToken)
    {
        await service.DeleteParameter(request.Id);

        return new DeleteParameterResult(request.Id);
    }
}
