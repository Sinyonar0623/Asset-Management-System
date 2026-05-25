using Asset.Data;
using Asset.Service.CommandHandlerService;
using Shared.Data.UnitOfWork;

namespace Asset.Assets.Features.AssetUnitFeature.CreateAssetUnit;

public class CreateAssetUnitHandler(
    IUnitOfWork<AssetDbContext> unitOfWork,
    IAssetUnitCommandHandlerService service) 
    : ICommandHandler<CreateAssetUnitCommand, CreateAssetUnitResult>
{
    private readonly IUnitOfWork<AssetDbContext> _unitOfWork = unitOfWork;
    private readonly IAssetUnitCommandHandlerService _service = service;

    public async Task<CreateAssetUnitResult> Handle(CreateAssetUnitCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var assetUnits = await _service.CreateAssetUnit(request.AssetUnits, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new CreateAssetUnitResult(assetUnits);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
