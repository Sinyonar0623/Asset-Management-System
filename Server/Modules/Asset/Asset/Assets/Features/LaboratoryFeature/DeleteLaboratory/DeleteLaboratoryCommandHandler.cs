using Asset.Data;
using Asset.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.LaboratoryFeature.DeleteLaboratory;

public class DeleteLaboratoryCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    ILaboratoryCommandHandlerService service) : ICommandHandler<DeleteLaboratoryCommand, DeleteLaboratoryResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly ILaboratoryCommandHandlerService _service = service;

    public async Task<DeleteLaboratoryResult> Handle(DeleteLaboratoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _service.DeleteLaboratory(request.Id, cancellationToken);

            if (result.IsSuccess)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new DeleteLaboratoryResult(result.IsSuccess, result.Message);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
