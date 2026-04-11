namespace Request.Dto;

public sealed record CreateRequestDto
{
    public string RequestNo { get; init; } = string.Empty;
    public string RequestType { get; init; } = string.Empty;
    public Guid TargetLaboratoryId { get; init; }
    public string RequestedAssetCategory { get; init; } = string.Empty;
    public Guid RequesterId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public RequestDetailDto? Detail { get; init; }
}
