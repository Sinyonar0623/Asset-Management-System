using Asset.Service;

namespace Asset.Assets.Features.AssetUnitFeature.CreateAssetUnit;

public class CreateAssetUnitHandler(IAssetUnitService _service) : ICommandHandler<CreateAssetUnitCommand, CreateAssetUnitResult>
{
    public async Task<CreateAssetUnitResult> Handle(CreateAssetUnitCommand request, CancellationToken cancellationToken)
    {
        var assetUnits = await _service.CreateAssetUnit(request.AssetUnits, cancellationToken);

        return new CreateAssetUnitResult(assetUnits);
    }
}
