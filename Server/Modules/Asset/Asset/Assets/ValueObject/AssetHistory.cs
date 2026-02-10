using System.Runtime.CompilerServices;

namespace Asset.Assets.ValueObject;

public class AssetHistory
{
    public string Purpose { get; }
    public string Remark { get; }
    public Guid ApproveBy { get; }
    public DateTime ApproveAt { get; }

    private AssetHistory(
        string purpose,
        string remark,
        Guid approveBy
    )
    {
        Purpose = purpose;
        Remark = remark;
        ApproveBy = approveBy;
    }

    public static AssetHistory Create(
        string purpose,
        string remark,
        Guid approveBy
    )
    {
        return new AssetHistory(
            purpose,
            remark,
            approveBy
        );
    }
}