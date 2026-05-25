using Asset.Data;
using Asset.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.AssetUnitFeature.UpdateAssetUnit;

public class UpdateAssetUnitCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    IAssetUnitCommandHandlerService service)
    : ICommandHandler<UpdateAssetUnitCommand, UpdateAssetUnitResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly IAssetUnitCommandHandlerService _service = service;

    public async Task<UpdateAssetUnitResult> Handle(UpdateAssetUnitCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var id = await _service.UpdateAssetUnit(request.Id, request.AssetUnit, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new UpdateAssetUnitResult(id);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
