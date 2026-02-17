using Auth.Authentication.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Repository;

namespace Auth.Data.Repository;

public class AuthRepository(AuthDbContext dbContext) : Repository<UserName, Guid>(dbContext), IAuthRepository
{
    private readonly AuthDbContext _context = dbContext;

    public async Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken cancellationToken = default)
    {
        return await _context.Auth.AsNoTracking()
            .AnyAsync(x => x.Username == username || x.Email == email, cancellationToken);
    }

    public async Task<UserRole?> GetRoleByCodeAsync(string roleCode, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .FirstOrDefaultAsync(x => x.RoleCode == roleCode, cancellationToken);
    }
}
