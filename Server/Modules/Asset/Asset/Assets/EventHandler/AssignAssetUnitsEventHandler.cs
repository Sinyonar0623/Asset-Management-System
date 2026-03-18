using Asset.Assets.Events;
using Asset.Service.EventHandlerService;
using MediatR;

namespace Asset.Assets.EventHandler;

public sealed class AssignAssetUnitsEventHandler(
    IAssetUnitEventHandlerService _service
)
    : INotificationHandler<AssignAssetUnitsEvent>
{
    public async Task Handle(AssignAssetUnitsEvent notification, CancellationToken cancellationToken)
    {
        var asset = notification.Asset;

        foreach (var unit in notification.AssetUnitId)
        {
            var assetUnit = await _service.GetAssetUnitById(unit, cancellationToken);

            assetUnit?.AssignAsset(asset);
        }
    }

}
