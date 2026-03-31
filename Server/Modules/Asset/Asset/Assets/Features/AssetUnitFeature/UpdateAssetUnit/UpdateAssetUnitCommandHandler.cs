using Asset.Service;
using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetUnitFeature.UpdateAssetUnit;

public class UpdateAssetUnitCommandHandler(IAssetUnitCommandHandlerService service)
    : ICommandHandler<UpdateAssetUnitCommand, UpdateAssetUnitResult>
{
    private readonly IAssetUnitCommandHandlerService _service = service;
    public async Task<UpdateAssetUnitResult> Handle(UpdateAssetUnitCommand request, CancellationToken cancellationToken)
    {
        var id = await _service.UpdateAssetUnit(request.Id, request.AssetUnit, cancellationToken);

        return new UpdateAssetUnitResult(id);
    }
}
