using Asset.Service;

namespace Asset.Assets.Features.AssetUnitFeature.UpdateAssetUnit;

public class UpdateAssetUnitCommandHandler(IAssetUnitService service)
    : ICommandHandler<UpdateAssetUnitCommand, UpdateAssetUnitResult>
{
    public async Task<UpdateAssetUnitResult> Handle(UpdateAssetUnitCommand request, CancellationToken cancellationToken)
    {
        var id = await service.UpdateAssetUnit(request.Id, request.AssetUnit, cancellationToken);

        return new UpdateAssetUnitResult(id);
    }
}
