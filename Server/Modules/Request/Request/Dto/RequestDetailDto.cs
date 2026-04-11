namespace Request.Dto;

public sealed record RequestDetailDto
{
    public string? Purpose { get; init; }
    public DateTime? BorrowFrom { get; init; }
    public DateTime? BorrowTo { get; init; }
    public string? IssueDescription { get; init; }
    public string? RetireReason { get; init; }
    public string? ExtraNote { get; init; }
}
