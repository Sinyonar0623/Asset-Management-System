namespace Asset.Assets.ValueObject;

public class AssetHistory
{
    public string Purpose { get; private set; } = null!;
    public string Remark { get; private set; } = null!;
    public Guid ApproveBy { get; private set; }
    public DateTime ApproveAt { get; private set; }

    private AssetHistory() {}

    private AssetHistory(
        string purpose,
        string remark,
        Guid approveBy,
        DateTime approveAt
    )
    {
        Purpose = purpose;
        Remark = remark;
        ApproveBy = approveBy;
        ApproveAt = approveAt;
    }

    public static AssetHistory Create(
        string purpose,
        string remark,
        Guid approveBy,
        DateTime? approveAt = null
    )
    {
        return new AssetHistory(
            purpose,
            remark,
            approveBy,
            approveAt ?? DateTime.UtcNow
        );
    }
}
