using Auth.Authentication.Model;
using Shared.Data;

namespace Auth.Data.Repository;

public interface IAuthRepository : IRepository<UserName, Guid>
{
    Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken cancellationToken = default);
    Task<UserName?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<UserName?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserRole?> GetRoleByCodeAsync(string roleCode, CancellationToken cancellationToken = default);
    Task<UserName?> GetByIdWithRoleAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<UserName>> GetUsersWithRolesAsync(CancellationToken cancellationToken = default);
    Task<List<UserName>> GetUsersByIdsAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken = default);
    Task<List<UserName>> GetUsersByRoleCodeAsync(string roleCode, CancellationToken cancellationToken = default);
    Task<Guid> GetHODId(CancellationToken cancellationToken = default);
}
