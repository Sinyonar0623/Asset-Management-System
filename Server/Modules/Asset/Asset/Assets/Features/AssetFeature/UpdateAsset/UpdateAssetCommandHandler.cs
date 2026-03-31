using Asset.Data;
using Asset.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.AssetFeature.UpdateAsset;

public class UpdateAssetCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    IAssetCommandHandlerService service) : ICommandHandler<UpdateAssetCommand, UpdateAssetResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly IAssetCommandHandlerService _service = service;

    public async Task<UpdateAssetResult> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var isSuccess = await _service.UpdateAsset(request.Id, request.Asset, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new UpdateAssetResult(isSuccess);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
