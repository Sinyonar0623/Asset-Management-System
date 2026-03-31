using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetUnitFeature.CreateAssetUnit;

public class CreateAssetUnitHandler(IAssetUnitCommandHandlerService service) 
    : ICommandHandler<CreateAssetUnitCommand, CreateAssetUnitResult>
{
    private readonly IAssetUnitCommandHandlerService _service = service;
    public async Task<CreateAssetUnitResult> Handle(CreateAssetUnitCommand request, CancellationToken cancellationToken)
    {
        var assetUnits = await _service.CreateAssetUnit(request.AssetUnits, cancellationToken);

        return new CreateAssetUnitResult(assetUnits);
    }
}
