using Shared.DDD;

namespace Reauest.Requests.Model;

public class RepairRequest : Aggregate<long>
{
    public Guid RequesterId { get; private set; }
    public string RequesterName { get; private set; } = null!;
    public long AssetId { get; private set; }
    public string AssetName { get; private set; } = null!;
    public string AssetRealWorldId { get; private set; } = null!;
    public string ProblemDescription { get; private set; } = null!;
    public string Status { get; private set; } = null!; // pending, approved, in_repair, completed, rejected
    public string? ApprovalRemark { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? TechnicianNote { get; private set; }

    private RepairRequest() { }

    private RepairRequest(
        Guid requesterId,
        string requesterName,
        long assetId,
        string assetName,
        string assetRealWorldId,
        string problemDescription)
    {
        RequesterId = requesterId;
        RequesterName = requesterName;
        AssetId = assetId;
        AssetName = assetName;
        AssetRealWorldId = assetRealWorldId;
        ProblemDescription = problemDescription;
        Status = "pending";
    }

    public static RepairRequest Create(
        Guid requesterId,
        string requesterName,
        long assetId,
        string assetName,
        string assetRealWorldId,
        string problemDescription)
    {
        return new RepairRequest(requesterId, requesterName, assetId, assetName, assetRealWorldId, problemDescription);
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

    public void StartRepair()
    {
        Status = "in_repair";
    }

    public void Complete(string? technicianNote)
    {
        Status = "completed";
        CompletedAt = DateTime.UtcNow;
        TechnicianNote = technicianNote;
    }
}
