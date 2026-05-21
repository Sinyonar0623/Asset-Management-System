using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data;

namespace Asset.Data.Repository.Read;

public class AssetUnitReadRepository(AssetDbContext dbContext)
    : BaseReadRepository<AssetUnit, Guid>(dbContext), IAssetUnitReadRepository
{
    private readonly AssetDbContext _context = dbContext;

    public async Task<List<AssetUnitDto>> GetAssetUnitsByAssetIdAsync(
        Guid assetId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .AsNoTracking()
            .Where(x => EF.Property<Guid?>(x, "AssetId") == assetId)
            .Select(x => new AssetUnitDto
            {
                Id = x.Id,
                AssetId = EF.Property<Guid?>(x, "AssetId"),
                AssetTag = x.AssetTag,
                SerialNo = x.SerialNo,
                Name = x.Name,
                Brand = x.Brand,
                AvailabilityStatus = x.AvailabilityStatus,
                OperationalStatus = x.OperationalStatus,
                Remark = x.Remark,
                ResponsibleUserId = x.ResponsibleUserId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AssetUnitDto?> GetAssetUnitDtoByIdAsync(
        Guid assetUnitId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .AsNoTracking()
            .Where(x => x.Id == assetUnitId)
            .Select(x => new AssetUnitDto
            {
                Id = x.Id,
                AssetId = EF.Property<Guid?>(x, "AssetId"),
                AssetTag = x.AssetTag,
                SerialNo = x.SerialNo,
                Name = x.Name,
                Brand = x.Brand,
                AvailabilityStatus = x.AvailabilityStatus,
                OperationalStatus = x.OperationalStatus,
                Remark = x.Remark,
                ResponsibleUserId = x.ResponsibleUserId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<AssetUnitDto>> GetUnassignedAssetUnitsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .AsNoTracking()
            .Where(x => EF.Property<Guid?>(x, "AssetId") == null)
            .Select(x => new AssetUnitDto
            {
                Id = x.Id,
                AssetId = EF.Property<Guid?>(x, "AssetId"),
                AssetTag = x.AssetTag,
                SerialNo = x.SerialNo,
                Name = x.Name,
                Brand = x.Brand,
                AvailabilityStatus = x.AvailabilityStatus,
                OperationalStatus = x.OperationalStatus,
                Remark = x.Remark,
                ResponsibleUserId = x.ResponsibleUserId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AssetUnitDetailDto?> GetAssetUnitDetailDtoByIdAsync(
        Guid assetUnitId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .AsNoTracking()
            .Where(x => x.Id == assetUnitId)
            .Select(assetUnit => new AssetUnitDetailDto
            {
                Unit = new AssetUnitDto
                {
                    Id = assetUnit.Id,
                    AssetId = EF.Property<Guid?>(assetUnit, "AssetId"),
                    AssetTag = assetUnit.AssetTag,
                    SerialNo = assetUnit.SerialNo,
                    Name = assetUnit.Name,
                    Brand = assetUnit.Brand,
                    AvailabilityStatus = assetUnit.AvailabilityStatus,
                    OperationalStatus = assetUnit.OperationalStatus,
                    Remark = assetUnit.Remark,
                    ResponsibleUserId = assetUnit.ResponsibleUserId
                },
                Images = assetUnit.Images.Select(image => new AssetUnitImageDto
                {
                    Id = image.Id,
                    AssetUnitId = assetUnit.Id,
                    ImageUrl = image.ImageUrl,
                    Description = image.Description,
                    FileName = image.FileName,
                    ContentType = image.ContentType,
                    FileSizeBytes = image.FileSizeBytes
                }).ToList(),
                Histories = assetUnit.Histories.Select(history => new AssetHistoryDto
                {
                    AssetUnitId = assetUnit.Id,
                    ActionType = history.ActionType,
                    FromAvailabilityStatus = history.FromAvailabilityStatus,
                    ToAvailabilityStatus = history.ToAvailabilityStatus,
                    FromOperationalStatus = history.FromOperationalStatus,
                    ToOperationalStatus = history.ToOperationalStatus,
                    FromResponsibleUserId = history.FromResponsibleUserId,
                    ToResponsibleUserId = history.ToResponsibleUserId,
                    PerformedBy = history.PerformedBy,
                    PerformedAt = history.PerformedAt,
                    ApprovedBy = history.ApprovedBy,
                    ApprovedAt = history.ApprovedAt,
                    ReferenceNo = history.ReferenceNo,
                    RequestId = history.RequestId,
                    Remark = history.Remark
                }).OrderByDescending(x => x.PerformedAt).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<AssetHistoryDto>> GetAssetHistoriesByAssetIdAsync(
        Guid assetId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AssetUnits
            .AsNoTracking()
            .Where(x => EF.Property<Guid?>(x, "AssetId") == assetId)
            .SelectMany(assetUnit => assetUnit.Histories.Select(history => new AssetHistoryDto
            {
                AssetUnitId = assetUnit.Id,
                ActionType = history.ActionType,
                FromAvailabilityStatus = history.FromAvailabilityStatus,
                ToAvailabilityStatus = history.ToAvailabilityStatus,
                FromOperationalStatus = history.FromOperationalStatus,
                ToOperationalStatus = history.ToOperationalStatus,
                FromResponsibleUserId = history.FromResponsibleUserId,
                ToResponsibleUserId = history.ToResponsibleUserId,
                PerformedBy = history.PerformedBy,
                PerformedAt = history.PerformedAt,
                ApprovedBy = history.ApprovedBy,
                ApprovedAt = history.ApprovedAt,
                ReferenceNo = history.ReferenceNo,
                RequestId = history.RequestId,
                Remark = history.Remark
            }))
            .OrderByDescending(x => x.PerformedAt)
            .ToListAsync(cancellationToken);
    }
}
