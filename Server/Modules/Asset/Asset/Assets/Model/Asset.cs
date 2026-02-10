using Asset.Assets.ValueObject;
using Shared.DDD;

namespace Asset.Assets.Model;

public class Asset : Aggregate<long>
{
    public string RealWorldId { get; private set; } = null!;
    public string Brand { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string SerialNo { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Type { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public long Amount { get; private set; }
    public string Remark { get; private set; } = null!;
    public Guid OwnerId { get; private set; }

    public AssetLaboratory Laboratory { get; private set; } = default!;

    private readonly List<AssetHistory> _histories = [];
    public IReadOnlyList<AssetHistory> Histories => _histories.AsReadOnly();

    private Asset() {}

    private Asset(string realWorldId,
        string brand,
        string name,
        string serialNo,
        string description,
        string type,
        string status,
        long amount,
        string remark,
        Guid ownerId)
    {
        RealWorldId = realWorldId;
        Brand = brand;
        Name = name;
        SerialNo = serialNo;
        Description = description;
        Type = type;
        Status = status;
        Amount = amount;
        Remark = remark;
        OwnerId = ownerId;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("SonarQube", "S107:Methods should not have too many parameters")]
    public static Asset Create(
        string realWorldId,
        string brand,
        string name,
        string serialNo,
        string description,
        string type,
        string status,
        string remark,
        long amount,
        Guid ownerId
    )
    {
        return new Asset(
            realWorldId,
            brand,
            name,
            serialNo,
            description,
            type,
            status,
            amount,
            remark,
            ownerId
        );
    }

    public void AssignLaboratory(AssetLaboratory laboratory)
    {
        Laboratory = laboratory;
    }

    public void AddHistory(AssetHistory history)
    {
        _histories.Add(history);
    }
}
