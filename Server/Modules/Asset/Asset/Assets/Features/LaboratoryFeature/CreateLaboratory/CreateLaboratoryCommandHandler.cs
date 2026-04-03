using Asset.Data;
using Asset.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.LaboratoryFeature.CreateLaboratory;

public class CreateLaboratoryCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    ILaboratoryCommandHandlerService service) : ICommandHandler<CreateLaboratoryCommand, CreateLaboratoryResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly ILaboratoryCommandHandlerService _service = service;

    public async Task<CreateLaboratoryResult> Handle(CreateLaboratoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var laboratoryId = await _service.CreateLaboratory(request.Laboratory, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new CreateLaboratoryResult(laboratoryId);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
