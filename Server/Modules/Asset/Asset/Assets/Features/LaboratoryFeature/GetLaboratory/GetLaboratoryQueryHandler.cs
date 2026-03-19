using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.LaboratoryFeature.GetLaboratory;

public class GetLaboratoryQueryHandler(ILaboratoryCommandHandlerService service)
    : IQueryHandler<GetLaboratoryQuery, GetLaboratoryResult>
{
    private readonly ILaboratoryCommandHandlerService _service = service;

    public async Task<GetLaboratoryResult> Handle(GetLaboratoryQuery request, CancellationToken cancellationToken)
    {
        var laboratories = await _service.GetLaboratories(cancellationToken);
        
        return new GetLaboratoryResult(laboratories);
    }
}
