using Microsoft.AspNetCore.Authorization;

namespace Shared.Security.Authorization;

public sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;
