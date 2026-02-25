using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;

namespace Asset.Data.Repository;

public interface IAssetRepository
{
    Task<Assets.Model.Asset?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Assets.Model.Asset?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Assets.Model.Asset> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, string? type, string? status, CancellationToken cancellationToken = default);
    Task AddAsync(Assets.Model.Asset asset, CancellationToken cancellationToken = default);
    Task UpdateAsync(Assets.Model.Asset asset, CancellationToken cancellationToken = default);
    Task DeleteAsync(Assets.Model.Asset asset, CancellationToken cancellationToken = default);
    Task<bool> ExistsByRealWorldIdAsync(string realWorldId, long? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySerialNoAsync(string serialNo, long? excludeId = null, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<AssetLaboratory?> GetLaboratoryByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AssetLaboratory>> GetAllLaboratoriesAsync(CancellationToken cancellationToken = default);
    Task AddLaboratoryAsync(AssetLaboratory laboratory, CancellationToken cancellationToken = default);
}
