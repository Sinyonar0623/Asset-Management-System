namespace Request.Requests.Model;

public class Request : Aggregate<Guid>
{
    public string RequestNo { get; private set; } = null!;
    public string RequestType { get; private set; } = null!;
    public Guid TargetLaboratoryId { get; private set; }
    public string RequestedAssetCategory { get; private set; } = null!;
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
        string requestNo,
        string requestType,
        Guid targetLaboratoryId,
        string requestedAssetCategory,
        Guid requesterId,
        string reason)
    {
        Id = Guid.NewGuid();
        RequestNo = requestNo;
        RequestType = requestType;
        TargetLaboratoryId = targetLaboratoryId;
        RequestedAssetCategory = requestedAssetCategory;
        RequesterId = requesterId;
        Reason = reason;
        Status = RequestStatusCodes.Pending;
        SubmittedOn = DateTime.UtcNow;
    }

    public static Request Create(
        string requestNo,
        string requestType,
        Guid targetLaboratoryId,
        string requestedAssetCategory,
        Guid requesterId,
        string reason)
    {
        return new Request(
            requestNo,
            requestType,
            targetLaboratoryId,
            requestedAssetCategory,
            requesterId,
            reason);
    }

    public void Update(
        string requestType,
        Guid targetLaboratoryId,
        string requestedAssetCategory,
        Guid requesterId,
        string reason)
    {
        RequestType = requestType;
        TargetLaboratoryId = targetLaboratoryId;
        RequestedAssetCategory = requestedAssetCategory;
        RequesterId = requesterId;
        Reason = reason;
    }

    public void AddOrIncreaseItem(Guid assetId, int quantityRequested, string? note = null)
    {
        var existing = _items.FirstOrDefault(x => x.AssetId == assetId);
        if (existing is null)
        {
            _items.Add(RequestItem.Create(assetId, quantityRequested, note));
            return;
        }

        existing.IncreaseQuantity(quantityRequested);
        existing.UpdateNote(note);
    }

    public void ChangeItemQuantity(Guid assetId, int quantityRequested)
    {
        var existing = _items.FirstOrDefault(x => x.AssetId == assetId)
            ?? throw new InvalidOperationException("Request item not found.");

        existing.ChangeQuantity(quantityRequested);
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
            var teacherStep = RequestTracking.Create(1, ApproverRoleCodes.Teacher, teacherApproverId);
            teacherStep.Skip("Requester is the laboratory teacher.");
            _trackings.Add(teacherStep);

            var hodStep = RequestTracking.Create(2, ApproverRoleCodes.Hod, hodApproverId);
            hodStep.Activate();
            _trackings.Add(hodStep);

            CurrentStepNo = 2;
            NextApproverId = hodApproverId;
            return;
        }

        if (teacherApproverId.HasValue)
        {
            var teacherStep = RequestTracking.Create(1, ApproverRoleCodes.Teacher, teacherApproverId);
            teacherStep.Activate();
            _trackings.Add(teacherStep);

            _trackings.Add(RequestTracking.Create(2, ApproverRoleCodes.Hod, hodApproverId));

            CurrentStepNo = 1;
            NextApproverId = teacherApproverId;
            return;
        }

        var onlyHodStep = RequestTracking.Create(1, ApproverRoleCodes.Hod, hodApproverId);
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
