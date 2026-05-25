using Auth.Dto;

namespace Auth.Authentication.Features.GetUserLookup;

public record GetUserLookupResult(List<UserLookupDto> Users);
