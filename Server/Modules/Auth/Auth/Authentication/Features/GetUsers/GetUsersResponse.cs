using Auth.Dto;

namespace Auth.Authentication.Features.GetUsers;

public record GetUsersResponse(List<UserDto> Users);
