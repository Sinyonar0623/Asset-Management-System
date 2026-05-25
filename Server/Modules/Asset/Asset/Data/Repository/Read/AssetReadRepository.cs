using Microsoft.EntityFrameworkCore;
using Asset.Assets.Model;
using Shared.Data;
using System.Data;

namespace Asset.Data.Repository.Read;

public class AssetReadRepository(AssetDbContext dbContext)
    : BaseReadRepository<Assets.Model.Asset, Guid>(dbContext), IAssetReadRepository
{
    private readonly AssetDbContext _context = dbContext;

    public async Task<long> GetAssetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AssetModels
            .AsNoTracking()
            .LongCountAsync(cancellationToken);
    }

    public async Task<long> GetAssetCountByLaboratoryIdAsync(Guid laboratoryId, CancellationToken cancellationToken = default)
    {
        return await _context.AssetModels
            .AsNoTracking()
            .LongCountAsync(x => EF.Property<Guid?>(x, "LaboratoryId") == laboratoryId, cancellationToken);
    }

    public async Task<bool> NameExistsInLaboratoryAsync(
        Guid laboratoryId,
        string name,
        Guid? excludeAssetModelId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Laboratories
            .AsNoTracking()
            .Where(x => x.Id == laboratoryId);

        if (excludeAssetModelId.HasValue)
        {
            query = query.Where(x => x.Id != excludeAssetModelId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetRequestedAssetIdsByRequesterAsync(
        Guid requesterId,
        CancellationToken cancellationToken = default)
    {
        var connection = _context.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT DISTINCT ri."AssetId"
                FROM request."RequestItems" ri
                INNER JOIN request."Requests" r ON r."Id" = ri."RequestId"
                WHERE r."RequesterId" = @requesterId
                  AND r."Status" IN ('PENDING', 'APPROVED')
                """;

            var parameter = command.CreateParameter();
            parameter.ParameterName = "requesterId";
            parameter.Value = requesterId;
            command.Parameters.Add(parameter);

            var assetIds = new List<Guid>();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                assetIds.Add(reader.GetGuid(0));
            }

            return assetIds;
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }
    }

    public async Task<Dictionary<Guid, string>> GetAvailabilityStatusesByAssetIdsAsync(
        IReadOnlyCollection<Guid> assetIds,
        CancellationToken cancellationToken = default)
    {
        var ids = assetIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (ids.Count == 0)
        {
            return [];
        }

        var unitStatuses = await _context.AssetUnits
            .AsNoTracking()
            .Where(x => EF.Property<Guid?>(x, "AssetId").HasValue
                        && ids.Contains(EF.Property<Guid?>(x, "AssetId")!.Value))
            .Select(x => new
            {
                AssetId = EF.Property<Guid?>(x, "AssetId")!.Value,
                x.AvailabilityStatus,
                x.OperationalStatus
            })
            .ToListAsync(cancellationToken);

        return unitStatuses
            .GroupBy(x => x.AssetId)
            .ToDictionary(
                x => x.Key,
                x =>
                {
                    if (x.Any(unit => unit.AvailabilityStatus == AssetUnitStatuses.Availability.InUse))
                    {
                        return AssetUnitStatuses.Availability.InUse;
                    }

                    if (x.Any(unit => unit.AvailabilityStatus == AssetUnitStatuses.Availability.Reserved))
                    {
                        return AssetUnitStatuses.Availability.Reserved;
                    }

                    if (x.Any(unit =>
                            unit.AvailabilityStatus == AssetUnitStatuses.Availability.Available
                            && unit.OperationalStatus == AssetUnitStatuses.Operational.Ready))
                    {
                        return AssetUnitStatuses.Availability.Available;
                    }

                    return AssetUnitStatuses.Availability.Unavailable;
                });
    }
}
