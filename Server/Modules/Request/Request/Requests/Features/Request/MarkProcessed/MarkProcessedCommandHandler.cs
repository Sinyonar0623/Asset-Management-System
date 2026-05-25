namespace Request.Requests.Features.Request.MarkProcessed;

public sealed class MarkProcessedCommandHandler(
    IUnitOfWork<RequestDbContext> unitOfWork,
    IRequestCommandHandlerService service
) : ICommandHandler<MarkProcessedCommand, MarkProcessedResult>
{
    private readonly IUnitOfWork<RequestDbContext> _unitOfWork = unitOfWork;
    private readonly IRequestCommandHandlerService _service = service;

    public async Task<MarkProcessedResult> Handle(MarkProcessedCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var isSuccess = await _service.MarkRequestProcessed(
                request.RequestId,
                request.ApproverId,
                request.ApproverRoleCode,
                request.Decision,
                request.Comment,
                request.AssetIds,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new MarkProcessedResult(isSuccess);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
