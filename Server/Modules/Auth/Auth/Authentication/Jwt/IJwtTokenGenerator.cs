using Auth.Dto;

namespace Auth.Authentication.Jwt;

public interface IJwtTokenGenerator
{
    (string AccessToken, int ExpiresInSeconds) GenerateToken(LoginUserDto user);
}
