namespace Request.Requests.Features.Request.CreateRequest;

public class CreateRequestCommandHandler(
    IUnitOfWork<RequestDbContext> unitOfWork,
    IRequestCommandHandlerService service)
    : ICommandHandler<CreateRequestCommand, CreateRequestResult>
{
    private readonly IUnitOfWork<RequestDbContext> _unitOfWork = unitOfWork;
    private readonly IRequestCommandHandlerService _service = service;

    public async Task<CreateRequestResult> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var requestId = await _service.CreateRequest(
                request.Request,
                request.RequesterId,
                request.RequesterRoleCode,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new CreateRequestResult(requestId);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            throw;
        }
    }
}
