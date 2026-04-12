using Auth.Authentication.Model;
using Auth.Dto;

namespace Auth.Service;

public interface IAuthService
{   
    Task<Guid> AddNewUser(UsernameDto user, CancellationToken cancellationToken);
    Task<LoginAttemptDto> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task<bool> LogoutAsync(Guid userId, CancellationToken cancellationToken);
    Task<Guid> GetHODId(CancellationToken cancellationToken);
}
