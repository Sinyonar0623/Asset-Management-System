using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.LaboratoryFeature.DeleteLaboratory;

public class DeleteLaboratoryCommandHandler(
    ILaboratoryCommandHandlerService service) : ICommandHandler<DeleteLaboratoryCommand, DeleteLaboratoryResult>
{
    private readonly ILaboratoryCommandHandlerService _service = service;

    public async Task<DeleteLaboratoryResult> Handle(DeleteLaboratoryCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteLaboratory(request.Id, cancellationToken);
        return new DeleteLaboratoryResult(result.IsSuccess, result.Message);
    }
}
