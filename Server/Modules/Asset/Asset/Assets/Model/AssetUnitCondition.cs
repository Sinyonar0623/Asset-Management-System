using Shared.DDD;

namespace Asset.Assets.Model;

public class AssetUnitCondition : Entity<Guid>
{
    public string Reason { get; private set; } = null!;
    public DateTime? EffectiveFrom { get; private set; }
    public bool IsEffectiveFromUnknown { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public bool IsEffectiveToUnknown { get; private set; }

    public AssetUnit AssetUnit { get; private set; } = default!;

    private AssetUnitCondition() {}

    private AssetUnitCondition(
        string reason,
        DateTime? effectiveFrom,
        bool isEffectiveFromUnknown,
        DateTime? effectiveTo,
        bool isEffectiveToUnknown)
    {
        Reason = reason;
        SetPeriod(effectiveFrom, isEffectiveFromUnknown, effectiveTo, isEffectiveToUnknown);
    }

    public static AssetUnitCondition Create(
        string reason,
        DateTime? effectiveFrom,
        bool isEffectiveFromUnknown,
        DateTime? effectiveTo,
        bool isEffectiveToUnknown)
    {
        return new AssetUnitCondition(
            reason,
            effectiveFrom,
            isEffectiveFromUnknown,
            effectiveTo,
            isEffectiveToUnknown);
    }

    public void Update(
        string reason,
        DateTime? effectiveFrom,
        bool isEffectiveFromUnknown,
        DateTime? effectiveTo,
        bool isEffectiveToUnknown)
    {
        Reason = reason;
        SetPeriod(effectiveFrom, isEffectiveFromUnknown, effectiveTo, isEffectiveToUnknown);
    }

    private void SetPeriod(
        DateTime? effectiveFrom,
        bool isEffectiveFromUnknown,
        DateTime? effectiveTo,
        bool isEffectiveToUnknown)
    {
        EffectiveFrom = isEffectiveFromUnknown ? null : effectiveFrom;
        IsEffectiveFromUnknown = isEffectiveFromUnknown;
        EffectiveTo = isEffectiveToUnknown ? null : effectiveTo;
        IsEffectiveToUnknown = isEffectiveToUnknown;
    }
}
