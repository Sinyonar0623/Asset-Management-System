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
        AvailabilityStatus = availabilityStatus;
        OperationalStatus = operationalStatus;
        Remark = remark;
        OwnerId = ownerId;
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
        AvailabilityStatus = availabilityStatus;
        OperationalStatus = operationalStatus;
    }

    public void Update(
        string assetTag,
        string serialNo,
        string availabilityStatus,
        string operationalStatus,
        string remark,
        Guid ownerId)
    {
        AssetTag = assetTag;
        SerialNo = serialNo;
        AvailabilityStatus = availabilityStatus;
        OperationalStatus = operationalStatus;
        Remark = remark;
        OwnerId = ownerId;
    }

    public void AddHistory(AssetHistory history)
    {
        _histories.Add(history);
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
}
