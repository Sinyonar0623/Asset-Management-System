using MassTransit;
using MediatR;
using Request.Requests.Events;
using Request.Requests.Model;
using Request.Service.EventHandlerService;
using Shared.Messaging.Integration.Command;
using Shared.Messaging.Integration.Response;
using Shared.Security;

namespace Request.Requests.EventHandler;

public sealed class CreateNewRequestEventHandler(
    IRequestClient<GetApproverIntegration> FetchApprover,
    IRequestClient<GetApproverHODIntegration> FetchHOD,
    IRequestEventHandlerService service
) : INotificationHandler<CreateNewRequestEvent>
{
    private readonly IRequestClient<GetApproverIntegration> _fetchApprover = FetchApprover;
    private readonly IRequestClient<GetApproverHODIntegration> _fetchHOD = FetchHOD;
    private readonly IRequestEventHandlerService _service = service;

    public async Task Handle(CreateNewRequestEvent notification, CancellationToken cancellationToken)
    {
        var hod = await _fetchHOD.GetResponse<GetApproverHODIntegrationResponse>(
            new GetApproverHODIntegration(),
            cancellationToken
        );

        if (string.Equals(notification.RequestType, RequestTypeCodes.Borrow, StringComparison.OrdinalIgnoreCase))
        {
            var approver = await _fetchApprover.GetResponse<GetApproverIntegrationResponse>(
                new GetApproverIntegration(notification.LabId),
                cancellationToken
            );

            await _service.AssignTaskToApprover(
                notification.ReqId,
                approver.Message.ApproverId,
                hod.Message.ApproverId,
                string.Equals(notification.RequesterRoleCode, RoleCodes.Teacher, StringComparison.OrdinalIgnoreCase)
                    && notification.RequesterId == approver.Message.ApproverId,
                cancellationToken);

            return;
        }

        await _service.AssignTaskToApprover(
            notification.ReqId,
            null,
            hod.Message.ApproverId,
            false,
            cancellationToken);
    }
}
