namespace Asset.Dto;

public sealed record AssetHistoryDto
{
    public Guid AssetUnitId { get; init; }
    public string ActionType { get; init; } = string.Empty;
    public string? FromAvailabilityStatus { get; init; }
    public string? ToAvailabilityStatus { get; init; }
    public string? FromOperationalStatus { get; init; }
    public string? ToOperationalStatus { get; init; }
    public Guid? FromResponsibleUserId { get; init; }
    public Guid? ToResponsibleUserId { get; init; }
    public Guid PerformedBy { get; init; }
    public DateTime PerformedAt { get; init; }
    public Guid? ApprovedBy { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public string? ReferenceNo { get; init; }
    public Guid? RequestId { get; init; }
    public string Remark { get; init; } = string.Empty;
}
