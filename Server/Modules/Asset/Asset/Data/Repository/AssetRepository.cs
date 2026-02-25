using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;

namespace Asset.Data.Repository;

public class AssetRepository(AssetDbContext dbContext) : IAssetRepository
{
    private readonly AssetDbContext _context = dbContext;

    public async Task<Assets.Model.Asset?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(a => a.Laboratory)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Assets.Model.Asset?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Include(a => a.Laboratory)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<(IEnumerable<Assets.Model.Asset> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, string? type, string? status, CancellationToken cancellationToken = default)
    {
        var query = _context.Assets
            .Include(a => a.Laboratory)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(a =>
                a.Name.ToLower().Contains(s) ||
                a.RealWorldId.ToLower().Contains(s) ||
                a.Brand.ToLower().Contains(s) ||
                a.SerialNo.ToLower().Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(a => a.Type == type);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(a => a.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Assets.Model.Asset asset, CancellationToken cancellationToken = default)
    {
        await _context.Assets.AddAsync(asset, cancellationToken);
    }

    public Task UpdateAsync(Assets.Model.Asset asset, CancellationToken cancellationToken = default)
    {
        _context.Assets.Update(asset);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Assets.Model.Asset asset, CancellationToken cancellationToken = default)
    {
        _context.Assets.Remove(asset);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByRealWorldIdAsync(string realWorldId, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Assets.AsNoTracking().Where(a => a.RealWorldId == realWorldId);
        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsBySerialNoAsync(string serialNo, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Assets.AsNoTracking().Where(a => a.SerialNo == serialNo);
        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AssetLaboratory?> GetLaboratoryByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<AssetLaboratory>().FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<AssetLaboratory>> GetAllLaboratoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<AssetLaboratory>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task AddLaboratoryAsync(AssetLaboratory laboratory, CancellationToken cancellationToken = default)
    {
        await _context.Set<AssetLaboratory>().AddAsync(laboratory, cancellationToken);
    }
}
