using Asset.Assets.Model;
using Asset.Data.Repository.Read;
using Asset.Data.Repository.Write;

namespace Asset.Service.EventHandlerService;

public class AssetUnitEventHandlerService(
    IAssetUnitWriteRepository assetUnitWriteRepository) : IAssetUnitEventHandlerService
{
    private readonly IAssetUnitWriteRepository _assetUnitWriteRepository = assetUnitWriteRepository;

    public async Task<AssetUnit?> GetAssetUnitById(Guid assetUnitId, CancellationToken cancellationToken = default)
    {
        var assetUnit = await _assetUnitWriteRepository.GetByIdAsync(assetUnitId, cancellationToken);

        return assetUnit;
    }

}
