using Shared.DDD;

namespace Parameter.Parameters.Model;

public class Parameter : Aggregate<long>
{
    public string Group { get; private set; } = default!;
    public string Value { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public bool Active { get; private set; }

    private Parameter() { }

    private Parameter(
        string group,
        string value,
        string description
    )
    {
        Group = group;
        Value = value;
        Description = description;
        Active = true;
    }

    public static Parameter Create(
        string group,
        string value,
        string description
    )
    {
        return new (group, value, description);
    }

    public void Disable()
    {
        Active = false;
    }

    public void Enable()
    {
        Active = true;
    }
}