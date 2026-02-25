using Asset.Data.Repository;
using Shared.CQRS;

namespace Asset.Assets.Features.CreateAsset;

public sealed class CreateAssetCommandHandler(IAssetRepository assetRepository)
    : ICommandHandler<CreateAssetCommand, CreateAssetResult>
{
    private readonly IAssetRepository _repository = assetRepository;

    public async Task<CreateAssetResult> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsByRealWorldIdAsync(request.RealWorldId, null, cancellationToken))
            throw new InvalidOperationException($"Asset with RealWorldId '{request.RealWorldId}' already exists.");

        if (await _repository.ExistsBySerialNoAsync(request.SerialNo, null, cancellationToken))
            throw new InvalidOperationException($"Asset with SerialNo '{request.SerialNo}' already exists.");

        var asset = Assets.Model.Asset.Create(
            request.RealWorldId,
            request.Brand,
            request.Name,
            request.SerialNo,
            request.Description,
            request.Type,
            "available",
            request.Remark,
            request.Amount,
            request.OwnerId
        );

        await _repository.AddAsync(asset, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new CreateAssetResult(asset.Id);
    }
}
