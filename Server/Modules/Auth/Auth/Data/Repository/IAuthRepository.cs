using Auth.Authentication.Model;
using Shared.Data.Repository;

namespace Auth.Data.Repository;

public interface IAuthRepository : IRepository<UserName, Guid>
{
    Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken cancellationToken = default);
    Task<UserName?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<UserName?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserRole?> GetRoleByCodeAsync(string roleCode, CancellationToken cancellationToken = default);
}
