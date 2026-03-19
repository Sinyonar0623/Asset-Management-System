using Asset.Data;
using Asset.Service.CommandHandlerService;
using Mapster;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.AssetFeature.AssignAssetUnit;

public class AssignAssetUnitCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    IAssetCommandHandlerService service
) : ICommandHandler<AssignAssetUnitCommand, AssignAssetUnitResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly IAssetCommandHandlerService _service = service;
    public async Task<AssignAssetUnitResult> Handle(AssignAssetUnitCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _service.AssignAssetUnitsAsync(request.AssetId, request.NewUnits, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return result.Adapt<AssignAssetUnitResult>();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            throw;
        }
    }
}