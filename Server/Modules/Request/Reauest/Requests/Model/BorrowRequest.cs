using Shared.DDD;

namespace Reauest.Requests.Model;

public class BorrowRequest : Aggregate<long>
{
    public Guid RequesterId { get; private set; }
    public string RequesterName { get; private set; } = null!;
    public long AssetId { get; private set; }
    public string AssetName { get; private set; } = null!;
    public string AssetRealWorldId { get; private set; } = null!;
    public DateTime BorrowDate { get; private set; }
    public DateTime ReturnDate { get; private set; }
    public string Purpose { get; private set; } = null!;
    public string Status { get; private set; } = null!; // pending, approved, rejected, returned
    public string? ApprovalRemark { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? ActualReturnDate { get; private set; }

    private BorrowRequest() { }

    private BorrowRequest(
        Guid requesterId,
        string requesterName,
        long assetId,
        string assetName,
        string assetRealWorldId,
        DateTime borrowDate,
        DateTime returnDate,
        string purpose)
    {
        RequesterId = requesterId;
        RequesterName = requesterName;
        AssetId = assetId;
        AssetName = assetName;
        AssetRealWorldId = assetRealWorldId;
        BorrowDate = borrowDate;
        ReturnDate = returnDate;
        Purpose = purpose;
        Status = "pending";
    }

    public static BorrowRequest Create(
        Guid requesterId,
        string requesterName,
        long assetId,
        string assetName,
        string assetRealWorldId,
        DateTime borrowDate,
        DateTime returnDate,
        string purpose)
    {
        return new BorrowRequest(requesterId, requesterName, assetId, assetName, assetRealWorldId, borrowDate, returnDate, purpose);
    }

    public void Approve(Guid approvedBy, string? remark)
    {
        Status = "approved";
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
        ApprovalRemark = remark;
    }

    public void Reject(Guid rejectedBy, string remark)
    {
        Status = "rejected";
        ApprovedBy = rejectedBy;
        ApprovedAt = DateTime.UtcNow;
        ApprovalRemark = remark;
    }

    public void MarkReturned()
    {
        Status = "returned";
        ActualReturnDate = DateTime.UtcNow;
    }
}
