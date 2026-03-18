using Asset.Service.CommandHandlerService;

namespace Asset.Assets.Features.AssetUnitFeature.DeleteAssetUnit;

public class DeleteAssetUnitCommandHandler(IAssetUnitCommandHandlerService service)
    : ICommandHandler<DeleteAssetUnitCommand, DeleteAssetUnitResult>
{
    private readonly IAssetUnitCommandHandlerService _service = service;
    public async Task<DeleteAssetUnitResult> Handle(DeleteAssetUnitCommand request, CancellationToken cancellationToken)
    {
        var id = await _service.DeleteAssetUnit(request.Id, cancellationToken);

        return new DeleteAssetUnitResult(id);
    }
}
