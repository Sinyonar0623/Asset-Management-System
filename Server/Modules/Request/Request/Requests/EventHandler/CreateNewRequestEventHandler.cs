using Mapster;
using MassTransit;
using MediatR;
using Request.Requests.Events;
using Request.Service.EventHandlerService;
using Shared.Messaging.Integration.Command;
using Shared.Messaging.Integration.Response;

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

        var approver = await _fetchApprover.GetResponse<GetApproverIntegrationResponse>(
            new GetApproverIntegration(notification.LabId),
            cancellationToken
        );

        var hod = await _fetchHOD.GetResponse<GetApproverHODIntegrationResponse>(
            new GetApproverHODIntegration(),
            cancellationToken
        );

        // TODO : STUDENT CASE ONLY
        var result = await _service.AssignTaskToApprover(notification.ReqId, 
            approver.Message.ApproverId, 
            hod.Message.ApproverId, 
            false, 
            cancellationToken);   
    }
}
