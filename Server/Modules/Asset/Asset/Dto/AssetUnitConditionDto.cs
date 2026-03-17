namespace Asset.Dto;

public sealed record AssetUnitConditionDto
{
    public string Reason { get; init; } = string.Empty;
    public DateTime? EffectiveFrom { get; init; }
    public bool IsEffectiveFromUnknown { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public bool IsEffectiveToUnknown { get; init; }
}
