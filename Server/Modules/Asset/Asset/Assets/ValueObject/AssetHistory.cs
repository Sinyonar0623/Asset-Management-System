namespace Asset.Assets.ValueObject;

public class AssetHistory
{
    public string ActionType { get; private set; } = null!;
    public string? FromAvailabilityStatus { get; private set; }
    public string? ToAvailabilityStatus { get; private set; }
    public string? FromOperationalStatus { get; private set; }
    public string? ToOperationalStatus { get; private set; }
    public Guid? FromOwnerId { get; private set; }
    public Guid? ToOwnerId { get; private set; }
    public Guid PerformedBy { get; private set; }
    public DateTime PerformedAt { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? ReferenceNo { get; private set; }
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
        Guid? fromOwnerId,
        Guid? toOwnerId,
        Guid? approvedBy,
        DateTime? approvedAt,
        string? referenceNo
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
        FromOwnerId = fromOwnerId;
        ToOwnerId = toOwnerId;
        ApprovedBy = approvedBy;
        ApprovedAt = approvedAt;
        ReferenceNo = referenceNo;
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
        Guid? fromOwnerId = null,
        Guid? toOwnerId = null,
        Guid? approvedBy = null,
        DateTime? approvedAt = null,
        string? referenceNo = null
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
            fromOwnerId,
            toOwnerId,
            approvedBy,
            approvedAt,
            referenceNo
        );
    }
}
