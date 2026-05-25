using Auth.Dto;

namespace Auth.Authentication.Features.GetUserLookup;

public record GetUserLookupResponse(List<UserLookupDto> Users);
