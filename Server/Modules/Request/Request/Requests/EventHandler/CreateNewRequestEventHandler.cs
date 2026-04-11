using Mapster;
using MassTransit;
using MediatR;
using Request.Requests.Events;
using Shared.Messaging.Integration.Command;
using Shared.Messaging.Integration.Response;

namespace Request.Requests.EventHandler;

public sealed class CreateNewRequestEventHandler(
    IRequestClient<CreateNewRequestIntegrationCommand> bus
) : INotificationHandler<CreateNewRequestEvent>
{
    private readonly IRequestClient<CreateNewRequestIntegrationCommand> _bus = bus;

    public async Task Handle(CreateNewRequestEvent notification, CancellationToken cancellationToken)
    {
        var integrationCommand = notification.Adapt<CreateNewRequestIntegrationCommand>();

        var response = await _bus.GetResponse<CreateNewRequestIntegrationCommandResponse>(
            integrationCommand,
            cancellationToken
        );

    }
}
