namespace Auth.Dto;

public record UsernameDto(
    string Username,
    string Email,
    string Password,
    string RoleCode = "00"
);
