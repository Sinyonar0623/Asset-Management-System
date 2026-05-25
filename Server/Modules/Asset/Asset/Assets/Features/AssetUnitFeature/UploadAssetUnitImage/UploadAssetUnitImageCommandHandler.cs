using Asset.Data;
using Asset.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.AssetUnitFeature.UploadAssetUnitImage;

public class UploadAssetUnitImageCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    IAssetUnitCommandHandlerService service)
    : ICommandHandler<UploadAssetUnitImageCommand, UploadAssetUnitImageResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly IAssetUnitCommandHandlerService _service = service;

    public async Task<UploadAssetUnitImageResult> Handle(
        UploadAssetUnitImageCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var images = await _service.AddAssetUnitImages(
                request.AssetUnitId,
                request.Images,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new UploadAssetUnitImageResult(images);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
