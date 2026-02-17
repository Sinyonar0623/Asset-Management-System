namespace Auth.Dto;

public sealed record LoginUserDto(
    Guid UserId,
    string Username,
    string Email,
    string RoleCode,
    string RoleName
);
