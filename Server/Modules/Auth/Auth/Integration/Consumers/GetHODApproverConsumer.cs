using Auth.Service;
using MassTransit;
using Shared.Messaging.Integration.Command;
using Shared.Messaging.Integration.Response;

namespace Auth.Integration.Consumers;

public sealed class GetHODApproverConsumer(
    IAuthService service
) : IConsumer<GetApproverHODIntegration>
{
    private readonly IAuthService _service = service;

    public async Task Consume(ConsumeContext<GetApproverHODIntegration> context)
    {
        var approverId = await _service.GetHODId(context.CancellationToken);
        await context.RespondAsync(new GetApproverHODIntegrationResponse(approverId));
    }
}
