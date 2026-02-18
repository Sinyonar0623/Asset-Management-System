using Shared.DDD;

namespace Asset.Assets.Model;

public class AssetModel : Aggregate<long>
{
    public long LaboratoryId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public bool IsAvailable { get; private set; }

    public Laboratory Laboratory { get; private set; } = default!;

    private AssetModel() {}

    private AssetModel(
        long laboratoryId,
        string name,
        string description,
        string category,
        bool isAvailable)
    {
        LaboratoryId = laboratoryId;
        Name = name;
        Description = description;
        Category = category;
        IsAvailable = isAvailable;
    }

    public static AssetModel Create(
        long laboratoryId,
        string name,
        string description,
        string category,
        bool isAvailable = false)
    {
        return new AssetModel(laboratoryId, name, description, category, isAvailable);
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }
}
