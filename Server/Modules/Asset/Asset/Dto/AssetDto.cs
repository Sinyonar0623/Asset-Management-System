namespace Asset.Dto;

public sealed record AssetDto
{
    public Guid? Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public bool? IsAvailable { get; init; }
    public string? AvailabilityStatus { get; init; }
    public string? Location { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
