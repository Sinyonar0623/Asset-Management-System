namespace Request.Dto;

public sealed record RequestDto
{
    public Guid Id { get; init; }
    public string RequestNo { get; init; } = string.Empty;
    public string RequestType { get; init; } = string.Empty;
    public Guid TargetLaboratoryId { get; init; }
    public string RequestedAssetCategory { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public Guid RequesterId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public int? CurrentStepNo { get; init; }
    public Guid? NextApproverId { get; init; }
    public DateTime SubmittedOn { get; init; }
    public DateTime? FinalizedOn { get; init; }
    public RequestDetailDto? Detail { get; init; }
    public List<RequestItemDto> Items { get; init; } = [];
    public List<RequestTrackingDto> Trackings { get; init; } = [];
}
