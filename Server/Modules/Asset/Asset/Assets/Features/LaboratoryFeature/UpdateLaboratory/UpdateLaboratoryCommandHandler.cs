using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.LaboratoryFeature.UpdateLaboratory;

public class UpdateLaboratoryCommandHandler(
    ILaboratoryCommandHandlerService service) : ICommandHandler<UpdateLaboratoryCommand, UpdateLaboratoryResult>
{
    private readonly ILaboratoryCommandHandlerService _service = service;

    public async Task<UpdateLaboratoryResult> Handle(UpdateLaboratoryCommand request, CancellationToken cancellationToken)
    {
        var isSuccess = await _service.UpdateLaboratory(request.Id, request.Laboratory, cancellationToken);
        
        return new UpdateLaboratoryResult(isSuccess);
    }
}
