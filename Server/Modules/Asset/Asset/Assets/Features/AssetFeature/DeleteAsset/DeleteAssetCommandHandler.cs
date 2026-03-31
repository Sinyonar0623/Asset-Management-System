using Asset.Data;
using Asset.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.AssetFeature.DeleteAsset;

public class DeleteAssetCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    IAssetCommandHandlerService service) : ICommandHandler<DeleteAssetCommand, DeleteAssetResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly IAssetCommandHandlerService _service = service;

    public async Task<DeleteAssetResult> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var isSuccess = await _service.DeleteAsset(request.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new DeleteAssetResult(isSuccess);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
