using Asset.Assets.ValueObject;
using Shared.DDD;

namespace Asset.Assets.Model;

public class AssetUnit : Aggregate<Guid>
{
    public string AssetTag { get; private set; } = null!;
    public string SerialNo { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Brand { get; private set; } = null!;
    public string AvailabilityStatus { get; private set; } = null!;
    public string OperationalStatus { get; private set; } = null!;
    public string Remark { get; private set; } = null!;
    public Guid? ResponsibleUserId { get; private set; }

    public Asset? Asset { get; private set; }
    public AssetUnitCondition? Condition { get; private set; }
    private readonly List<AssetUnitImage> _images = [];
    public IReadOnlyList<AssetUnitImage> Images => _images.AsReadOnly();

    private readonly List<AssetHistory> _histories = [];
    public IReadOnlyList<AssetHistory> Histories => _histories.AsReadOnly();

    private AssetUnit() {}

    private AssetUnit(
        string assetTag,
        string serialNo,
        string name,
        string brand,
        string availabilityStatus,
        string operationalStatus,
        string remark,
        Guid? responsibleUserId)
    {
        AssetTag = assetTag;
        SerialNo = serialNo;
        Name = name;
        Brand = brand;
        Remark = remark;
        ResponsibleUserId = responsibleUserId;
        SetStatuses(availabilityStatus, operationalStatus);
    }

    public static AssetUnit Create(
        string assetTag,
        string serialNo,
        string name,
        string brand,
        string availabilityStatus,
        string operationalStatus,
        string remark,
        Guid? responsibleUserId)
    {
        return new AssetUnit(
            assetTag,
            serialNo,
            name,
            brand,
            availabilityStatus,
            operationalStatus,
            remark,
            responsibleUserId);
    }

    public void ChangeStatuses(string availabilityStatus, string operationalStatus)
    {
        SetStatuses(availabilityStatus, operationalStatus);
    }

    public void Reserve(
        Guid performedBy,
        Guid? requestId = null,
        string? remark = null)
    {
        var fromAvailabilityStatus = AvailabilityStatus;
        var fromOperationalStatus = OperationalStatus;
        var fromResponsibleUserId = ResponsibleUserId;

        ResponsibleUserId = null;
        SetStatuses(AssetUnitStatuses.Availability.Reserved, OperationalStatus);

        AddHistory(
            "RESERVE",
            remark ?? "Reserved for request.",
            performedBy,
            fromAvailabilityStatus: fromAvailabilityStatus,
            toAvailabilityStatus: AvailabilityStatus,
            fromOperationalStatus: fromOperationalStatus,
            toOperationalStatus: OperationalStatus,
            fromResponsibleUserId: fromResponsibleUserId,
            toResponsibleUserId: ResponsibleUserId,
            approvedBy: performedBy == Guid.Empty ? null : performedBy,
            approvedAt: DateTime.UtcNow,
            requestId: requestId);
    }

    public void MarkInUse(
        Guid responsibleUserId,
        Guid performedBy,
        Guid? requestId = null,
        string? remark = null)
    {
        var fromAvailabilityStatus = AvailabilityStatus;
        var fromOperationalStatus = OperationalStatus;
        var fromResponsibleUserId = ResponsibleUserId;

        ResponsibleUserId = responsibleUserId;
        SetStatuses(AssetUnitStatuses.Availability.InUse, AssetUnitStatuses.Operational.Ready);

        AddHistory(
            "MARK_IN_USE",
            remark ?? "Marked as in use.",
            performedBy,
            fromAvailabilityStatus: fromAvailabilityStatus,
            toAvailabilityStatus: AvailabilityStatus,
            fromOperationalStatus: fromOperationalStatus,
            toOperationalStatus: OperationalStatus,
            fromResponsibleUserId: fromResponsibleUserId,
            toResponsibleUserId: ResponsibleUserId,
            approvedBy: performedBy == Guid.Empty ? null : performedBy,
            approvedAt: DateTime.UtcNow,
            requestId: requestId);
    }

    public void Release(
        Guid performedBy,
        Guid? requestId = null,
        string? remark = null,
        string actionType = "RELEASE")
    {
        var fromAvailabilityStatus = AvailabilityStatus;
        var fromOperationalStatus = OperationalStatus;
        var fromResponsibleUserId = ResponsibleUserId;

        ResponsibleUserId = null;
        SetStatuses(AssetUnitStatuses.Availability.Available, AssetUnitStatuses.Operational.Ready);

        AddHistory(
            actionType,
            remark ?? "Released back to available.",
            performedBy,
            fromAvailabilityStatus: fromAvailabilityStatus,
            toAvailabilityStatus: AvailabilityStatus,
            fromOperationalStatus: fromOperationalStatus,
            toOperationalStatus: OperationalStatus,
            fromResponsibleUserId: fromResponsibleUserId,
            toResponsibleUserId: ResponsibleUserId,
            approvedBy: performedBy == Guid.Empty ? null : performedBy,
            approvedAt: DateTime.UtcNow,
            requestId: requestId);
    }

    public void AddUpdateHistory(
        Guid performedBy,
        string fromAvailabilityStatus,
        string toAvailabilityStatus,
        string fromOperationalStatus,
        string toOperationalStatus,
        Guid? fromResponsibleUserId,
        Guid? toResponsibleUserId,
        string? remark = null)
    {
        if (fromAvailabilityStatus == toAvailabilityStatus
            && fromOperationalStatus == toOperationalStatus
            && fromResponsibleUserId == toResponsibleUserId)
        {
            return;
        }

        AddHistory(
            "UPDATE",
            remark ?? "Asset unit status updated.",
            performedBy,
            fromAvailabilityStatus: fromAvailabilityStatus,
            toAvailabilityStatus: toAvailabilityStatus,
            fromOperationalStatus: fromOperationalStatus,
            toOperationalStatus: toOperationalStatus,
            fromResponsibleUserId: fromResponsibleUserId,
            toResponsibleUserId: toResponsibleUserId);
    }

    public void Update(
        string assetTag,
        string serialNo,
        string name,
        string brand,
        string availabilityStatus,
        string operationalStatus,
        string remark,
        Guid? responsibleUserId)
    {
        AssetTag = assetTag;
        SerialNo = serialNo;
        Name = name;
        Brand = brand;
        Remark = remark;
        ResponsibleUserId = responsibleUserId;
        SetStatuses(availabilityStatus, operationalStatus);
    }

    public bool IsReadyForUse()
    {
        return AvailabilityStatus == AssetUnitStatuses.Availability.Available
               && OperationalStatus == AssetUnitStatuses.Operational.Ready;
    }

    public void AddHistory(AssetHistory history)
    {
        _histories.Add(history);
    }

    public AssetUnitImage AddImage(
        string imageUrl,
        string? description = null,
        string? fileName = null,
        string? contentType = null,
        long? fileSizeBytes = null,
        bool isPrimary = false)
    {
        var image = AssetUnitImage.Create(
            imageUrl,
            description,
            fileName,
            contentType,
            fileSizeBytes,
            isPrimary);

        if (isPrimary)
        {
            foreach (var existingImage in _images)
            {
                existingImage.MarkAsPrimary(false);
            }
        }

        _images.Add(image);
        return image;
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);
        if (image is null)
        {
            return;
        }

        _images.Remove(image);
    }

    public void AddHistory(
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
        Guid? requestId = null)
    {
        _histories.Add(AssetHistory.Create(
            actionType,
            remark,
            performedBy,
            performedAt,
            fromAvailabilityStatus,
            toAvailabilityStatus,
            fromOperationalStatus,
            toOperationalStatus,
            fromResponsibleUserId,
            toResponsibleUserId,
            approvedBy,
            approvedAt,
            referenceNo,
            requestId));
    }

    public void SetCondition(
        string reason,
        DateTime? conditionFrom,
        bool isConditionFromUnknown,
        DateTime? conditionTo,
        bool isConditionToUnknown)
    {
        if (Condition is null)
        {
            Condition = AssetUnitCondition.Create(
                reason,
                conditionFrom,
                isConditionFromUnknown,
                conditionTo,
                isConditionToUnknown);
            return;
        }

        Condition.Update(
            reason,
            conditionFrom,
            isConditionFromUnknown,
            conditionTo,
            isConditionToUnknown);
    }

    public void ClearCondition()
    {
        Condition = null;
    }

    public void AssignAsset(Asset asset)
    {
        Asset = asset;
    }

    public void UnassignAsset()
    {
        Asset = null;
    }

    private void SetStatuses(string availabilityStatus, string operationalStatus)
    {
        AvailabilityStatus = NormalizeAvailabilityStatus(availabilityStatus);
        OperationalStatus = NormalizeOperationalStatus(operationalStatus);
    }

    private static string NormalizeAvailabilityStatus(string availabilityStatus)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(availabilityStatus)
            ? AssetUnitStatuses.Availability.PendingActivation
            : availabilityStatus.Trim().ToUpperInvariant();

        if (!AssetUnitStatuses.IsValidAvailability(normalizedStatus))
        {
            throw new ArgumentException($"Unsupported availability status: {availabilityStatus}", nameof(availabilityStatus));
        }

        return normalizedStatus;
    }

    private static string NormalizeOperationalStatus(string operationalStatus)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(operationalStatus)
            ? AssetUnitStatuses.Operational.Ready
            : operationalStatus.Trim().ToUpperInvariant();

        if (!AssetUnitStatuses.IsValidOperational(normalizedStatus))
        {
            throw new ArgumentException($"Unsupported operational status: {operationalStatus}", nameof(operationalStatus));
        }

        return normalizedStatus;
    }
}
