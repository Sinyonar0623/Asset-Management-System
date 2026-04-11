namespace Request.Dto;

public sealed record RequestTrackingDto
{
    public int StepNo { get; init; }
    public string RequiredRoleCode { get; init; } = string.Empty;
    public Guid? AssignedApproverId { get; init; }
    public string Status { get; init; } = string.Empty;
    public Guid? ActionByUserId { get; init; }
    public DateTime? ActionOn { get; init; }
    public string? Comment { get; init; }
    public bool IsCurrent { get; init; }
}
