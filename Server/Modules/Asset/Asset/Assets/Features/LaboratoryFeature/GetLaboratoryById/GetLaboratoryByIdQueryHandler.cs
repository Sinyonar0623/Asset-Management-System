using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.LaboratoryFeature.GetLaboratoryById;

public class GetLaboratoryByIdQueryHandler(ILaboratoryCommandHandlerService service)
    : IQueryHandler<GetLaboratoryByIdQuery, GetLaboratoryByIdResult>
{
    private readonly ILaboratoryCommandHandlerService _service = service;

    public async Task<GetLaboratoryByIdResult> Handle(GetLaboratoryByIdQuery request, CancellationToken cancellationToken)
    {
        var laboratory = await _service.GetLaboratoryById(request.Id, cancellationToken);
        return new GetLaboratoryByIdResult(laboratory);
    }
}
