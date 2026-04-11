using Request.Data;
using Request.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Request.Requests.Features.Request.UpdateRequest;

public class UpdateRequestCommandHandler(
    IUnitOfWork<RequestDbContext> unitOfWork,
    IRequestCommandHandlerService service)
    : ICommandHandler<UpdateRequestCommand, UpdateRequestResult>
{
    private readonly IUnitOfWork<RequestDbContext> _unitOfWork = unitOfWork;
    private readonly IRequestCommandHandlerService _service = service;

    public async Task<UpdateRequestResult> Handle(UpdateRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var isSuccess = await _service.UpdateRequest(request.Id, request.Request, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new UpdateRequestResult(isSuccess);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
