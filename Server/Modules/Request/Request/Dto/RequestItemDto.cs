namespace Request.Dto;

public sealed record RequestItemDto
{
    public Guid AssetId { get; init; }
    public int QuantityRequested { get; init; }
    public int? QuantityApproved { get; init; }
    public string? Note { get; init; }
}
