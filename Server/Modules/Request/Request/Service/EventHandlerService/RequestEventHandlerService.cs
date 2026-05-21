namespace Request.Service.EventHandlerService;

public class RequestEventHandlerService
(
    IRequestWriteRepository requestWriteRepository
) : IRequestEventHandlerService
{
    private readonly IRequestWriteRepository _writeRepository = requestWriteRepository;

    public async Task<bool> AssignTaskToApprover(
        Guid requestId,
        Guid? teacherApproverId,
        Guid HODApproverId,
        bool requesterIsTeacherOfTargetLab,
        CancellationToken cancellationToken)
    {
        var request = await _writeRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Request with id {requestId} was not found.");

        request.SetupApprovalFlow(HODApproverId, 
            teacherApproverId, 
            requesterIsTeacherOfTargetLab);

        return true;
    }
}
