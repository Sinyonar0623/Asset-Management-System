using Asset.Data;
using Asset.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.AssetUnitFeature.DeleteAssetUnit;

public class DeleteAssetUnitCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    IAssetUnitCommandHandlerService service)
    : ICommandHandler<DeleteAssetUnitCommand, DeleteAssetUnitResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly IAssetUnitCommandHandlerService _service = service;

    public async Task<DeleteAssetUnitResult> Handle(DeleteAssetUnitCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _service.DeleteAssetUnit(request.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new DeleteAssetUnitResult(result);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
