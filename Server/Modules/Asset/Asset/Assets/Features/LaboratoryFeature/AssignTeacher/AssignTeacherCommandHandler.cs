using Asset.Data;
using Asset.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.LaboratoryFeature.AssignTeacher;

public class AssignTeacherCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    ILaboratoryCommandHandlerService service) : ICommandHandler<AssignTeacherCommand, AssignTeacherResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly ILaboratoryCommandHandlerService _service = service;

    public async Task<AssignTeacherResult> Handle(AssignTeacherCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var isSuccess = await _service.AssignTeacher(request.LaboratoryId, request.TeacherId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new AssignTeacherResult(isSuccess);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
