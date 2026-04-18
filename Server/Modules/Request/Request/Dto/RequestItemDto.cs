namespace Request.Dto;

public sealed record RequestItemDto
{
    public Guid AssetId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
