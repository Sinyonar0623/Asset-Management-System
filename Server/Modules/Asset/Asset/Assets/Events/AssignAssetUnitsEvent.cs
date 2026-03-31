using Shared.DDD;

namespace Asset.Assets.Events;

public sealed record AssignAssetUnitsEvent (
    Model.Asset Asset,
    List<Guid> AssetUnitId
) : IDomainEvent;