namespace Asset.Dto;

public record AssetDto(
    long Id,
    string RealWorldId,
    string Brand,
    string Name,
    string SerialNo,
    string Description,
    string Type,
    string Status,
    long Amount,
    string Remark,
    Guid OwnerId,
    AssetLaboratoryDto? Laboratory,
    IReadOnlyList<AssetComponentDto> Components
);

public record AssetLaboratoryDto(
    long Id,
    string LaboratoryName,
    string RoomNo,
    Guid TeacherId,
    string Description
);

public record AssetSummaryDto(
    long Id,
    string RealWorldId,
    string Brand,
    string Name,
    string SerialNo,
    string Type,
    string Status,
    long Amount,
    string? LaboratoryName,
    string? RoomNo
);

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
