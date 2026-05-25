namespace Auth.Dto;

public sealed record UserLookupDto(
    Guid UserId,
    string Username);
