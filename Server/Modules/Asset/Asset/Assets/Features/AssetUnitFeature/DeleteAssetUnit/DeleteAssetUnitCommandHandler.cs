using Asset.Service;

namespace Asset.Assets.Features.AssetUnitFeature.DeleteAssetUnit;

public class DeleteAssetUnitCommandHandler(IAssetUnitService service)
    : ICommandHandler<DeleteAssetUnitCommand, DeleteAssetUnitResult>
{
    public async Task<DeleteAssetUnitResult> Handle(DeleteAssetUnitCommand request, CancellationToken cancellationToken)
    {
        var id = await service.DeleteAssetUnit(request.Id, cancellationToken);

        return new DeleteAssetUnitResult(id);
    }
}
