namespace Request.Requests.Model;

public class RequestTracking
{
    public int StepNo { get; private set; }
    public string RequiredRoleCode { get; private set; } = null!;
    public Guid? AssignedApproverId { get; private set; }
    public string Status { get; private set; } = null!;
    public Guid? ActionByUserId { get; private set; }
    public DateTime? ActionOn { get; private set; }
    public string? Comment { get; private set; }
    public bool IsCurrent { get; private set; }

    private RequestTracking() {}

    private RequestTracking(
        int stepNo,
        string requiredRoleCode,
        Guid? assignedApproverId)
    {
        StepNo = stepNo;
        RequiredRoleCode = requiredRoleCode;
        AssignedApproverId = assignedApproverId;
        Status = TrackingStatusCodes.Waiting;
        IsCurrent = false;
    }

    public static RequestTracking Create(
        int stepNo,
        string requiredRoleCode,
        Guid? assignedApproverId)
    {
        return new RequestTracking(stepNo, requiredRoleCode, assignedApproverId);
    }

    public void Activate()
    {
        Status = TrackingStatusCodes.Pending;
        ActionByUserId = null;
        ActionOn = null;
        Comment = null;
        IsCurrent = true;
    }

    public void Deactivate()
    {
        IsCurrent = false;
    }

    public void Approve(Guid actionByUserId, string? comment)
    {
        Status = TrackingStatusCodes.Approved;
        ActionByUserId = actionByUserId;
        ActionOn = DateTime.UtcNow;
        Comment = comment;
        IsCurrent = false;
    }

    public void Reject(Guid actionByUserId, string? comment)
    {
        Status = TrackingStatusCodes.Rejected;
        ActionByUserId = actionByUserId;
        ActionOn = DateTime.UtcNow;
        Comment = comment;
        IsCurrent = false;
    }

    public void Skip(string? comment = null)
    {
        Status = TrackingStatusCodes.Skipped;
        Comment = comment;
        ActionOn = DateTime.UtcNow;
        IsCurrent = false;
    }
}
