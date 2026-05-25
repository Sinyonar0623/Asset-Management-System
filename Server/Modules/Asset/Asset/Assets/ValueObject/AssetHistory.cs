namespace Asset.Assets.ValueObject;

public class AssetHistory
{
    public string ActionType { get; private set; } = null!;
    public string? FromAvailabilityStatus { get; private set; }
    public string? ToAvailabilityStatus { get; private set; }
    public string? FromOperationalStatus { get; private set; }
    public string? ToOperationalStatus { get; private set; }
    public Guid? FromResponsibleUserId { get; private set; }
    public Guid? ToResponsibleUserId { get; private set; }
    public Guid PerformedBy { get; private set; }
    public DateTime PerformedAt { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? ReferenceNo { get; private set; }
    public Guid? RequestId { get; private set; }
    public string Remark { get; private set; } = null!;

    private AssetHistory() {}

    private AssetHistory(
        string actionType,
        string remark,
        Guid performedBy,
        DateTime performedAt,
        string? fromAvailabilityStatus,
        string? toAvailabilityStatus,
        string? fromOperationalStatus,
        string? toOperationalStatus,
        Guid? fromResponsibleUserId,
        Guid? toResponsibleUserId,
        Guid? approvedBy,
        DateTime? approvedAt,
        string? referenceNo,
        Guid? requestId
    )
    {
        ActionType = actionType;
        Remark = remark;
        PerformedBy = performedBy;
        PerformedAt = performedAt;
        FromAvailabilityStatus = fromAvailabilityStatus;
        ToAvailabilityStatus = toAvailabilityStatus;
        FromOperationalStatus = fromOperationalStatus;
        ToOperationalStatus = toOperationalStatus;
        FromResponsibleUserId = fromResponsibleUserId;
        ToResponsibleUserId = toResponsibleUserId;
        ApprovedBy = approvedBy;
        ApprovedAt = approvedAt;
        ReferenceNo = referenceNo;
        RequestId = requestId;
    }

    public static AssetHistory Create(
        string actionType,
        string remark,
        Guid performedBy,
        DateTime? performedAt = null,
        string? fromAvailabilityStatus = null,
        string? toAvailabilityStatus = null,
        string? fromOperationalStatus = null,
        string? toOperationalStatus = null,
        Guid? fromResponsibleUserId = null,
        Guid? toResponsibleUserId = null,
        Guid? approvedBy = null,
        DateTime? approvedAt = null,
        string? referenceNo = null,
        Guid? requestId = null
    )
    {
        return new AssetHistory(
            actionType,
            remark,
            performedBy,
            performedAt ?? DateTime.UtcNow,
            fromAvailabilityStatus,
            toAvailabilityStatus,
            fromOperationalStatus,
            toOperationalStatus,
            fromResponsibleUserId,
            toResponsibleUserId,
            approvedBy,
            approvedAt,
            referenceNo,
            requestId
        );
    }
}
