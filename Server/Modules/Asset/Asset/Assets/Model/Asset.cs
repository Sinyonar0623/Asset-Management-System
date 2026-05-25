using Shared.DDD;

namespace Asset.Assets.Model;

public class Asset : Aggregate<Guid>
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public bool IsAvailable { get; private set; }

    public Laboratory? Laboratory { get; private set; }

    private Asset() {}

    private Asset(
        string name,
        string description,
        string category,
        bool isAvailable)
    {
        Name = name;
        Description = description;
        Category = category;
        IsAvailable = isAvailable;
    }

    public static Asset Create(
        string name,
        string description,
        string category,
        bool isAvailable = true
    )
    {
        return new Asset( name, description, category, isAvailable);
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }

    public void Update(
        string name,
        string description,
        string category
    )
    {
        Name = name;
        Description = description;
        Category = category;
    }

    public void AssignLaboratory (Laboratory lab)
    {
        Laboratory = lab;
    }
}
