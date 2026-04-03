using Shared.DDD;

namespace Asset.Assets.Events;

public sealed record AssignLaboratoryEvent
(
    Guid AssetId,
    Model.Laboratory Lab
) : IDomainEvent;
