using Shared.DDD;

namespace Asset.Assets.Events;

public sealed record GetApproverIdEvent(
    Guid LaboratoryId
) : IDomainEvent;