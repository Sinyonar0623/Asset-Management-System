using Asset.Assets.ValueObject;
using Shared.DDD;

namespace Asset.Assets.Model;

public class AssetUnit : Aggregate<long>
{
    public long AssetModelId { get; private set; }
    public string AssetTag { get; private set; } = null!;
    public string SerialNo { get; private set; } = null!;
    public string AvailabilityStatus { get; private set; } = null!;
    public string OperationalStatus { get; private set; } = null!;
    public string Remark { get; private set; } = null!;
    public Guid OwnerId { get; private set; }

    public AssetModel AssetModel { get; private set; } = default!;

    private readonly List<AssetHistory> _histories = [];
    public IReadOnlyList<AssetHistory> Histories => _histories.AsReadOnly();

    private AssetUnit() {}

    private AssetUnit(
        long assetModelId,
        string assetTag,
        string serialNo,
        string availabilityStatus,
        string operationalStatus,
        string remark,
        Guid ownerId)
    {
        AssetModelId = assetModelId;
        AssetTag = assetTag;
        SerialNo = serialNo;
        AvailabilityStatus = availabilityStatus;
        OperationalStatus = operationalStatus;
        Remark = remark;
        OwnerId = ownerId;
    }

    public static AssetUnit Create(
        long assetModelId,
        string assetTag,
        string serialNo,
        string availabilityStatus,
        string operationalStatus,
        string remark,
        Guid ownerId)
    {
        return new AssetUnit(
            assetModelId,
            assetTag,
            serialNo,
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
        long assetModelId,
        string assetTag,
        string serialNo,
        string availabilityStatus,
        string operationalStatus,
        string remark,
        Guid ownerId)
    {
        AssetModelId = assetModelId;
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
}
