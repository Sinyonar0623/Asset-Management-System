using Asset.Data;
using Asset.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.AssetFeature.AssignLaboratory;

public class AssignLaboratoryCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    ILaboratoryCommandHandlerService service
) : ICommandHandler<AssignLaboratoryCommand, AssignLaboratoryResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly ILaboratoryCommandHandlerService _service = service;
    public async Task<AssignLaboratoryResult> Handle(AssignLaboratoryCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var isSuccess = await _service.AssignLaboratory(command.AssetId, command.LabId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new AssignLaboratoryResult(isSuccess);

        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            throw;
        }
    }
}
