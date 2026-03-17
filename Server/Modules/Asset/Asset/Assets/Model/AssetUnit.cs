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
    public Guid OwnerId { get; private set; }

    public Asset Asset { get; private set; } = default!;
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
        Guid ownerId)
    {
        AssetTag = assetTag;
        SerialNo = serialNo;
        Name = name;
        Brand = brand;
        Remark = remark;
        OwnerId = ownerId;
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
        Guid ownerId)
    {
        return new AssetUnit(
            assetTag,
            serialNo,
            name,
            brand,
            availabilityStatus,
            operationalStatus,
            remark,
            ownerId);
    }

    public void ChangeStatuses(string availabilityStatus, string operationalStatus)
    {
        SetStatuses(availabilityStatus, operationalStatus);
    }

    public void Update(
        string assetTag,
        string serialNo,
        string name,
        string brand,
        string availabilityStatus,
        string operationalStatus,
        string remark,
        Guid ownerId)
    {
        AssetTag = assetTag;
        SerialNo = serialNo;
        Name = name;
        Brand = brand;
        Remark = remark;
        OwnerId = ownerId;
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

    public void AddImage(
        string imageUrl,
        string? fileName = null,
        string? contentType = null,
        long? fileSizeBytes = null,
        bool isPrimary = false)
    {
        var image = AssetUnitImage.Create(
            imageUrl,
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
        Guid? fromOwnerId = null,
        Guid? toOwnerId = null,
        Guid? approvedBy = null,
        DateTime? approvedAt = null,
        string? referenceNo = null)
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
            fromOwnerId,
            toOwnerId,
            approvedBy,
            approvedAt,
            referenceNo));
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

    public void AssignAssets(Asset asset)
    {
        Asset = asset;
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
