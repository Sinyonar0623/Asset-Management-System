using Shared.Security;

namespace Request.Requests.Model;

public class Request : Aggregate<Guid>
{
    public string RequestType { get; private set; } = null!;
    public Guid TargetLaboratoryId { get; private set; }
    public string Status { get; private set; } = null!;
    public Guid RequesterId { get; private set; }
    public string Reason { get; private set; } = null!;
    public int? CurrentStepNo { get; private set; }
    public Guid? NextApproverId { get; private set; }
    public DateTime SubmittedOn { get; private set; }
    public DateTime? FinalizedOn { get; private set; }
    public uint RowVersion { get; private set; }

    public RequestDetail? Detail { get; private set; }

    private readonly List<RequestItem> _items = [];
    public IReadOnlyList<RequestItem> Items => _items.AsReadOnly();

    private readonly List<RequestTracking> _trackings = [];
    public IReadOnlyList<RequestTracking> Trackings => _trackings.AsReadOnly();

    private Request() {}

    private Request(
        string requestType,
        Guid targetLaboratoryId,
        Guid requesterId,
        string reason)
    {
        Id = Guid.NewGuid();
        RequestType = requestType;
        TargetLaboratoryId = targetLaboratoryId;
        RequesterId = requesterId;
        Reason = reason;
        Status = RequestStatusCodes.Pending;
        SubmittedOn = DateTime.UtcNow;
    }

    public static Request Create(
        string requestType,
        Guid targetLaboratoryId,
        Guid requesterId,
        string reason)
    {
        return new Request(
            requestType,
            targetLaboratoryId,
            requesterId,
            reason);
    }

    public void Update(
        string requestType,
        Guid targetLaboratoryId,
        string reason)
    {
        RequestType = requestType;
        TargetLaboratoryId = targetLaboratoryId;
        Reason = reason;
    }

    public void AddItem(Guid assetId)
    {
        var existing = _items.FirstOrDefault(x => x.AssetId == assetId);
        if (existing is not null)
        {
            return;
        }

        _items.Add(RequestItem.Create(assetId));
    }

    public void RemoveItem(Guid assetId)
    {
        _items.RemoveAll(x => x.AssetId == assetId);
    }

    public void SetOrUpdateDetail(
        string? purpose,
        DateTime? borrowFrom,
        DateTime? borrowTo,
        string? issueDescription,
        string? retireReason,
        string? extraNote)
    {
        if (Detail is null)
        {
            Detail = RequestDetail.Create(
                purpose,
                borrowFrom,
                borrowTo,
                issueDescription,
                retireReason,
                extraNote);
            return;
        }

        Detail.Update(
            purpose,
            borrowFrom,
            borrowTo,
            issueDescription,
            retireReason,
            extraNote);
    }

    public void AddTracking(RequestTracking tracking)
    {
        _trackings.Add(tracking);
    }

    public void SetupApprovalFlow(Guid hodApproverId, Guid? teacherApproverId, bool requesterIsTeacherOfTargetLab)
    {
        _trackings.Clear();

        if (requesterIsTeacherOfTargetLab && teacherApproverId.HasValue)
        {
            var teacherStep = RequestTracking.Create(1, RoleCodes.Teacher, teacherApproverId);
            teacherStep.Skip("Requester is the laboratory teacher.");
            _trackings.Add(teacherStep);

            var hodStep = RequestTracking.Create(2, RoleCodes.Hod, hodApproverId);
            hodStep.Activate();
            _trackings.Add(hodStep);

            CurrentStepNo = 2;
            NextApproverId = hodApproverId;
            return;
        }

        if (teacherApproverId.HasValue)
        {
            var teacherStep = RequestTracking.Create(1, RoleCodes.Teacher, teacherApproverId);
            teacherStep.Activate();
            _trackings.Add(teacherStep);

            _trackings.Add(RequestTracking.Create(2, RoleCodes.Hod, hodApproverId));

            CurrentStepNo = 1;
            NextApproverId = teacherApproverId;
            return;
        }

        var onlyHodStep = RequestTracking.Create(1, RoleCodes.Hod, hodApproverId);
        onlyHodStep.Activate();
        _trackings.Add(onlyHodStep);

        CurrentStepNo = 1;
        NextApproverId = hodApproverId;
    }

    public void ActivateStep(int stepNo, Guid? nextApproverId)
    {
        CurrentStepNo = stepNo;
        NextApproverId = nextApproverId;

        foreach (var tracking in _trackings.Where(x => x.IsCurrent))
        {
            tracking.Deactivate();
        }

        var current = _trackings.FirstOrDefault(x => x.StepNo == stepNo);
        current?.Activate();
    }

    public bool ReassignHODApprover(Guid oldApproverId, Guid newApproverId)
    {
        var changed = false;

        foreach (var tracking in _trackings.Where(x =>
                     string.Equals(x.RequiredRoleCode, RoleCodes.Hod, StringComparison.OrdinalIgnoreCase)
                     && x.Status is TrackingStatusCodes.Waiting or TrackingStatusCodes.Pending))
        {
            changed = tracking.ReassignApprover(oldApproverId, newApproverId) || changed;
        }

        if (NextApproverId == oldApproverId
            && _trackings.Any(x =>
                x.IsCurrent
                && string.Equals(x.RequiredRoleCode, RoleCodes.Hod, StringComparison.OrdinalIgnoreCase)
                && x.AssignedApproverId == newApproverId))
        {
            NextApproverId = newApproverId;
            changed = true;
        }

        return changed;
    }

    public void MarkApproved()
    {
        Status = RequestStatusCodes.Approved;
        FinalizedOn = DateTime.UtcNow;
        CurrentStepNo = null;
        NextApproverId = null;
    }

    public void MarkRejected()
    {
        Status = RequestStatusCodes.Rejected;
        FinalizedOn = DateTime.UtcNow;
        CurrentStepNo = null;
        NextApproverId = null;
    }

    public void MarkCancelled()
    {
        Status = RequestStatusCodes.Cancelled;
        FinalizedOn = DateTime.UtcNow;
        CurrentStepNo = null;
        NextApproverId = null;
    }

    public void MarkCompleted()
    {
        Status = RequestStatusCodes.Completed;
        FinalizedOn = DateTime.UtcNow;
    }
}
