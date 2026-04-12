using Asset.Assets.Events;
using MediatR;

namespace Asset.Assets.EventHandler;

public sealed class GetApproverIdEventHandler(

) : INotificationHandler<GetApproverIdEvent>
{
    public Task Handle(GetApproverIdEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}