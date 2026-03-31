namespace Asset.Dto;

public sealed record AssetUnitDto
{
    public Guid? Id { get; init; }
    public Guid? AssetId { get; init; }
    public string AssetTag { get; init; } = string.Empty;
    public string SerialNo { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;
    public string AvailabilityStatus { get; init; } = string.Empty;
    public string OperationalStatus { get; init; } = string.Empty;
    public string Remark { get; init; } = string.Empty;
    public Guid? OwnerId { get; init; }
}
