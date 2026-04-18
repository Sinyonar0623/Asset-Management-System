using Auth.Authentication.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Security;

namespace Auth.Data.Repository;

public class AuthRepository(AuthDbContext dbContext) : BaseRepository<UserName, Guid>(dbContext), IAuthRepository
{
    private readonly AuthDbContext _context = dbContext;

    public async Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken cancellationToken = default)
    {
        return await _context.UserName.AsNoTracking()
            .AnyAsync(x => x.Username == username || x.Email == email, cancellationToken);
    }

    public async Task<UserName?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.UserName
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<UserName?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await _context.UserName
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Username == userName, cancellationToken);
    }

    public async Task<Guid> GetHODId(CancellationToken cancellationToken = default)
    {
        var hodUserId = await _context.UserName
            .AsNoTracking()
            .Include(x => x.Role)
            .Where(x => x.Role.RoleCode == RoleCodes.Hod)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (hodUserId == Guid.Empty)
        {
            throw new KeyNotFoundException("HOD user was not found.");
        }

        return hodUserId;
    }

    public async Task<UserRole?> GetRoleByCodeAsync(string roleCode, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .FirstOrDefaultAsync(x => x.RoleCode == roleCode, cancellationToken);
    }
}

