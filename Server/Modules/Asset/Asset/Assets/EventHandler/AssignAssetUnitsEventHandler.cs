using Asset.Assets.Events;
using Asset.Service.EventHandlerService;
using MediatR;

namespace Asset.Assets.EventHandler;

public sealed class AssignAssetUnitsEventHandler(
    IAssetUnitEventHandlerService service
)
    : INotificationHandler<AssignAssetUnitsEvent>
{
    private readonly IAssetUnitEventHandlerService _service = service;
    public async Task Handle(AssignAssetUnitsEvent notification, CancellationToken cancellationToken)
    {
        var asset = notification.Asset;

        await _service.AssignAssetUnit(asset, notification.AssetUnitId, cancellationToken);
    }

}
