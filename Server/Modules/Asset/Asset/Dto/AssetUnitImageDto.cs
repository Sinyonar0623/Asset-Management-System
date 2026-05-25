namespace Asset.Dto;

public sealed record AssetUnitImageDto
{
    public Guid Id { get; init; }
    public Guid AssetUnitId { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? FileName { get; init; }
    public string? ContentType { get; init; }
    public long? FileSizeBytes { get; init; }
}
