using Asset.Data;
using Asset.Service;
using Asset.Service.CommandHandlerService;
using Mapster;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.AssetFeature.CreateAsset;

public class CreateAssetCommandHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    IAssetCommandHandlerService service
    ) : ICommandHandler<CreateAssetCommand, CreateAssetResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly IAssetCommandHandlerService _service = service;
    public async Task<CreateAssetResult> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var result = await _service.CreateAsset(request.Asset, request.Units, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _unitOfWork.CommitTransactionAsync(cancellationToken);

        return result.Adapt<CreateAssetResult>();
    }
}