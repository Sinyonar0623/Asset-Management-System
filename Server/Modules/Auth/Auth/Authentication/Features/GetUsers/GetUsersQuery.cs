using Shared.CQRS;

namespace Auth.Authentication.Features.GetUsers;

public record GetUsersQuery() : IQuery<GetUsersResult>;
