using Auth.Authentication.Model;
using Auth.Dto;

namespace Auth.Service;

public interface IAuthService
{   
    Task<Guid> AddNewUser(UsernameDto user, CancellationToken cancellationToken);
}