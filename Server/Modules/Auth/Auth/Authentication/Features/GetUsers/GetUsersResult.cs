using Auth.Dto;

namespace Auth.Authentication.Features.GetUsers;

public record GetUsersResult(List<UserDto> Users);
