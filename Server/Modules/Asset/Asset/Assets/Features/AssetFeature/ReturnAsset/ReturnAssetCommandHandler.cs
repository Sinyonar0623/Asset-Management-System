using Asset.Data;
using Asset.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.AssetFeature.ReturnAsset;

public sealed class ReturnAssetCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    IAssetCommandHandlerService service)
    : ICommandHandler<ReturnAssetCommand, ReturnAssetResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly IAssetCommandHandlerService _service = service;

    public async Task<ReturnAssetResult> Handle(ReturnAssetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var isSuccess = await _service.ReturnVisibleAssetAsync(
                request.AssetId,
                request.UserId,
                request.RoleCode,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new ReturnAssetResult(isSuccess);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
