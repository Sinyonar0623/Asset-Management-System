namespace Auth.Dto;

public sealed record UserDto(
    Guid UserId,
    string Username,
    string Email,
    string RoleCode,
    string RoleName);
