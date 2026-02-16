using Shared.DDD;

namespace Asset.Assets.Model;

public class AssetComponent : Entity<long>
{
    public string RealWorldId { get; private set; } = null!;
    public string Brand { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string SerialNo { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Type { get; private set; } = null!;
    public string Remark { get; private set; } = null!;

    private AssetComponent() { }

    private AssetComponent(
        string realWorldId,
        string brand,
        string name,
        string serialNo,
        string description,
        string type,
        string remark
    )
    {
        RealWorldId = realWorldId;
        Brand = brand;
        Name = name;
        SerialNo = serialNo;
        Description = description;
        Type = type;
        Remark = remark;
    }

    public static AssetComponent Create(
        string realWorldId,
        string brand,
        string name,
        string serialNo,
        string description,
        string type,
        string remark
    )
    {
        return new AssetComponent(
            realWorldId,
            brand,
            name,
            serialNo,
            description,
            type,
            remark
        );
    }
}