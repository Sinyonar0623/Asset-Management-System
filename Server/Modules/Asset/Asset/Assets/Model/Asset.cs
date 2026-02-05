using Shared.DDD;

namespace Asset.Assets.Model;

public class Asset : Aggregate<Guid>
{
    public string RealWorldId { get; private set; }
    public string Brand { get; private set; }
    public string Name { get; private set; }
    public string SerialNo { get; private set; }
    public string Description { get; private set; }
    public string Type { get; private set; }
    public string Status { get; private set; }
    public long Amount { get; private set; }
    public string Remark { get; private set; }

    private Asset(string realWorldId,
        string brand,
        string name,
        string serialNo,
        string description,
        string type,
        string status,
        long amount,
        string remark)
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
        long amount
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
            remark
        );
    }
}