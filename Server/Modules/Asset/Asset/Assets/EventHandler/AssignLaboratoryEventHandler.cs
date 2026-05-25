using Asset.Assets.Events;
using Asset.Data.Repository.Write;
using MediatR;

namespace Asset.Assets.EventHandler;

public sealed class AssignLaboratoryEventHandler(
    IAssetWriteRepository assetWriteRepository
) : INotificationHandler<AssignLaboratoryEvent>
{
    private readonly IAssetWriteRepository _assetWriteRepository = assetWriteRepository;

    public async Task Handle(AssignLaboratoryEvent notification, CancellationToken cancellationToken)
    {
        var asset = await _assetWriteRepository.GetByIdAsync(notification.AssetId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset with id {notification.AssetId} was not found.");

        asset.AssignLaboratory(notification.Lab);
    }
}
