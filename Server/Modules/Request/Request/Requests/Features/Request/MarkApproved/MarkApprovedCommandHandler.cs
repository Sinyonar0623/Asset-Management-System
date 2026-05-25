namespace Request.Requests.Features.Request.MarkApproved;

public sealed class MarkApprovedCommandHandler(
    IUnitOfWork<RequestDbContext> UnitOfWork,
    IRequestCommandHandlerService Service
) : ICommandHandler<MarkApprovedCommand, MarkApprovedResult>
{
    private readonly IUnitOfWork<RequestDbContext> _unitOfWork = UnitOfWork;
    private readonly IRequestCommandHandlerService _service = Service;

    public async Task<MarkApprovedResult> Handle(MarkApprovedCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var result_approved = await _service.MarkRequestApproved(request.RequestId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new MarkApprovedResult(result_approved);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            throw;
        }
    }
}