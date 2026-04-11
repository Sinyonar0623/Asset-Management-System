using Request.Data;
using Request.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Request.Requests.Features.Request.DeleteRequest;

public class DeleteRequestCommandHandler(
    IUnitOfWork<RequestDbContext> unitOfWork,
    IRequestCommandHandlerService service)
    : ICommandHandler<DeleteRequestCommand, DeleteRequestResult>
{
    private readonly IUnitOfWork<RequestDbContext> _unitOfWork = unitOfWork;
    private readonly IRequestCommandHandlerService _service = service;

    public async Task<DeleteRequestResult> Handle(DeleteRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var isSuccess = await _service.DeleteRequest(request.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new DeleteRequestResult(isSuccess);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
