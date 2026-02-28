using Shared.DDD;

namespace Request.Requests.Model;

public class RequestDetail : Entity<long>
{
    public string? Purpose { get; private set; }
    public DateTime? BorrowFrom { get; private set; }
    public DateTime? BorrowTo { get; private set; }
    public string? IssueDescription { get; private set; }
    public string? RetireReason { get; private set; }
    public string? ExtraNote { get; private set; }

    public Request Request { get; private set; } = default!;

    private RequestDetail() {}

    private RequestDetail(
        string? purpose,
        DateTime? borrowFrom,
        DateTime? borrowTo,
        string? issueDescription,
        string? retireReason,
        string? extraNote)
    {
        Purpose = purpose;
        BorrowFrom = borrowFrom;
        BorrowTo = borrowTo;
        IssueDescription = issueDescription;
        RetireReason = retireReason;
        ExtraNote = extraNote;
    }

    public static RequestDetail Create(
        string? purpose,
        DateTime? borrowFrom,
        DateTime? borrowTo,
        string? issueDescription,
        string? retireReason,
        string? extraNote)
    {
        return new RequestDetail(
            purpose,
            borrowFrom,
            borrowTo,
            issueDescription,
            retireReason,
            extraNote);
    }

    public void Update(
        string? purpose,
        DateTime? borrowFrom,
        DateTime? borrowTo,
        string? issueDescription,
        string? retireReason,
        string? extraNote)
    {
        Purpose = purpose;
        BorrowFrom = borrowFrom;
        BorrowTo = borrowTo;
        IssueDescription = issueDescription;
        RetireReason = retireReason;
        ExtraNote = extraNote;
    }
}
