using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.LaboratoryFeature.CreateLaboratory;

public class CreateLaboratoryCommandHandler(
    ILaboratoryCommandHandlerService service) : ICommandHandler<CreateLaboratoryCommand, CreateLaboratoryResult>
{
    private readonly ILaboratoryCommandHandlerService _service = service;

    public async Task<CreateLaboratoryResult> Handle(CreateLaboratoryCommand request, CancellationToken cancellationToken)
    {
        var laboratoryId = await _service.CreateLaboratory(request.Laboratory, cancellationToken);
        return new CreateLaboratoryResult(laboratoryId);
    }
}
