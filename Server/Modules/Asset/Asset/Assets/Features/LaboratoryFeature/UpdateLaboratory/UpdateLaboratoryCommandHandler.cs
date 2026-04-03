using Asset.Data;
using Asset.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.LaboratoryFeature.UpdateLaboratory;

public class UpdateLaboratoryCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    ILaboratoryCommandHandlerService service) : ICommandHandler<UpdateLaboratoryCommand, UpdateLaboratoryResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly ILaboratoryCommandHandlerService _service = service;

    public async Task<UpdateLaboratoryResult> Handle(UpdateLaboratoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var isSuccess = await _service.UpdateLaboratory(request.Id, request.Laboratory, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new UpdateLaboratoryResult(isSuccess);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
