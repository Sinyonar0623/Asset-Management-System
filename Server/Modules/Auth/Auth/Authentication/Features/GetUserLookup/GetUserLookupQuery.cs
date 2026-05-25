using Shared.CQRS;

namespace Auth.Authentication.Features.GetUserLookup;

public record GetUserLookupQuery(IReadOnlyCollection<Guid> UserIds) : IQuery<GetUserLookupResult>;
