using Shared.CQRS;

namespace Auth.Authentication.Features.GetHOD;

public record GetHODQuery() : IQuery<GetHODResult>;
