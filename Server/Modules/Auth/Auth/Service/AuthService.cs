using Auth.Authentication.Jwt;
using Auth.Authentication.Model;
using Auth.Data;
using Auth.Data.Repository;
using Auth.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared.Data.UnitOfWork;

namespace Auth.Service;

public class AuthService(
    IAuthRepository authRepository,
    IUnitOfWork<AuthDbContext> unitOfWork,
    IPasswordHasher<UserName> passwordHasher,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly IPasswordHasher<UserName> _hasher = passwordHasher;
    private readonly IAuthRepository _repository = authRepository;
    private readonly IUnitOfWork<AuthDbContext> _unitOfWork = unitOfWork;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Guid> AddNewUser(UsernameDto user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Username)
            || string.IsNullOrWhiteSpace(user.Email)
            || string.IsNullOrWhiteSpace(user.Password))
        {
            throw new ArgumentException("Username, email and password are required.");
        }

        var username = user.Username.Trim();
        var email = user.Email.Trim();
        var roleCode = string.IsNullOrWhiteSpace(user.RoleCode) ? "00" : user.RoleCode.Trim();

        if (await _repository.ExistsByUsernameOrEmailAsync(username, email, cancellationToken))
        {
            throw new InvalidOperationException("Username or email already exists.");
        }

        var role = await _repository.GetRoleByCodeAsync(roleCode, cancellationToken) ?? throw new InvalidOperationException("Role not found.");

        var newUser = UserName.Create(user.Username, user.Email);

        newUser.SetPasswordHash(_hasher.HashPassword(newUser, user.Password));

        newUser.AssignRole(role);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await _repository.AddAsync(newUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        return newUser.Id;
    }

    public async Task<LoginAttemptDto> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return new LoginAttemptDto(false, null, "Email and password are required.", true, false);
        }

        var user = await _repository.GetByEmailAsync(email.Trim(), cancellationToken);
        if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash) || user.Role is null)
        {
            return new LoginAttemptDto(false, null, "Invalid email or password.", false, false);
        }

        var verified = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verified == PasswordVerificationResult.Failed)
        {
            return new LoginAttemptDto(false, null, "Invalid email or password.", false, false);
        }

        if (user.Session.HasValue && user.SessionActiveOn.HasValue)
        {
            var activeWindowMinutes = Math.Max(1, _jwtOptions.AccessTokenMinutes);
            var sessionExpiredAt = user.SessionActiveOn.Value.AddMinutes(activeWindowMinutes);

            if (sessionExpiredAt > DateTime.UtcNow)
            {
                return new LoginAttemptDto(false, null, "User is already logged in.", false, true);
            }

            user.ClearSession();
        }

        user.SetSession();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var loginUser = new LoginUserDto(
            user.Id,
            user.Username,
            user.Email,
            user.Role.RoleCode,
            user.Role.RoleName
        );

        return new LoginAttemptDto(true, loginUser, null, false, false);
    }

    public async Task<bool> LogoutAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return false;
        }

        user.ClearSession();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
