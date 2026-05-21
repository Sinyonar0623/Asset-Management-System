namespace Request.Service.EventHandlerService;

public interface IRequestEventHandlerService
{
    Task<bool> AssignTaskToApprover(
        Guid requestId,
        Guid? teacherApproverId,
        Guid HODApproverId,
        bool requesterIsTeacherOfTargetLab,
        CancellationToken cancellationToken);
}
