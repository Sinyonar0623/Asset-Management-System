using Auth.Authentication.Jwt;
using Auth.Authentication.Model;
using Auth.Data;
using Auth.Data.Repository;
using Auth.Dto;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared.Data.UnitOfWork;
using Shared.Messaging.Integration.Command;
using Shared.Messaging.Integration.Response;
using Shared.Security;

namespace Auth.Service;

public class AuthService(
    IAuthRepository authRepository,
    IUnitOfWork<AuthDbContext> unitOfWork,
    IPasswordHasher<UserName> passwordHasher,
    IOptions<JwtOptions> jwtOptions,
    IRequestClient<ReassignHODApproverCommand> reassignHODApproverClient) : IAuthService
{
    private readonly IPasswordHasher<UserName> _hasher = passwordHasher;
    private readonly IAuthRepository _repository = authRepository;
    private readonly IUnitOfWork<AuthDbContext> _unitOfWork = unitOfWork;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IRequestClient<ReassignHODApproverCommand> _reassignHODApproverClient = reassignHODApproverClient;

    public async Task<Guid> AddNewUser(UsernameDto user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Username)
            || string.IsNullOrWhiteSpace(user.Email)
            || string.IsNullOrWhiteSpace(user.Password)
            || string.IsNullOrWhiteSpace(user.RoleCode))
        {
            throw new ArgumentException("Username, email, password and role code are required.");
        }

        var username = user.Username.Trim();
        var email = user.Email.Trim();
        var roleCode = user.RoleCode.Trim().ToUpperInvariant();

        if (await _repository.ExistsByUsernameOrEmailAsync(username, email, cancellationToken))
        {
            throw new InvalidOperationException("Username or email already exists.");
        }

        var role = await _repository.GetRoleByCodeAsync(roleCode, cancellationToken)
            ?? throw new InvalidOperationException($"Role '{roleCode}' was not found.");
        var teacherRole = roleCode == RoleCodes.Hod
            ? await _repository.GetRoleByCodeAsync(RoleCodes.Teacher, cancellationToken)
                ?? throw new InvalidOperationException("Teacher role was not found.")
            : null;
        var currentHods = roleCode == RoleCodes.Hod
            ? await _repository.GetUsersByRoleCodeAsync(RoleCodes.Hod, cancellationToken)
            : [];

        var newUser = UserName.Create(username, email);

        newUser.SetPasswordHash(_hasher.HashPassword(newUser, user.Password));

        newUser.AssignRole(role);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var currentHod in currentHods)
            {
                currentHod.AssignRole(teacherRole!);
                currentHod.ClearSession();
            }

            await _repository.AddAsync(newUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        await ReassignHODApproversAsync(currentHods, newUser.Id, cancellationToken);

        return newUser.Id;
    }

    public async Task<List<TeacherDto>> GetAllUserAsTeacher(CancellationToken cancellationToken)
    {
        var teachers = await _repository.FindAsync(
            u => u.Role.RoleCode == RoleCodes.Teacher || u.Role.RoleCode == RoleCodes.Hod,
            u => new TeacherDto(u.Id, u.Username),
            cancellationToken);

        return teachers.ToList();
    }

    public async Task<List<UserDto>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _repository.GetUsersWithRolesAsync(cancellationToken);

        return users.Select(MapToUserDto).ToList();
    }

    public async Task<List<UserLookupDto>> GetUserLookup(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken)
    {
        var distinctUserIds = userIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (distinctUserIds.Count == 0)
        {
            return [];
        }

        var users = await _repository.GetUsersByIdsAsync(distinctUserIds, cancellationToken);

        return users
            .Select(user => new UserLookupDto(user.Id, user.Username))
            .ToList();
    }

    public async Task<UserDto> UpdateUserRole(
        Guid userId,
        string roleCode,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(roleCode))
        {
            throw new ArgumentException("Role code is required.", nameof(roleCode));
        }

        var normalizedRoleCode = roleCode.Trim().ToUpperInvariant();
        var user = await _repository.GetByIdWithRoleAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id {userId} was not found.");

        if (user.Role is null)
        {
            throw new InvalidOperationException("User has no role.");
        }

        if (user.Role.RoleCode == normalizedRoleCode)
        {
            return MapToUserDto(user);
        }

        if (user.Role.RoleCode == RoleCodes.Hod && normalizedRoleCode != RoleCodes.Hod)
        {
            throw new InvalidOperationException("Assign another user as HOD before changing the current HOD role.");
        }

        var role = await _repository.GetRoleByCodeAsync(normalizedRoleCode, cancellationToken)
            ?? throw new InvalidOperationException($"Role '{normalizedRoleCode}' was not found.");
        var teacherRole = normalizedRoleCode == RoleCodes.Hod
            ? await _repository.GetRoleByCodeAsync(RoleCodes.Teacher, cancellationToken)
                ?? throw new InvalidOperationException("Teacher role was not found.")
            : null;
        var currentHods = normalizedRoleCode == RoleCodes.Hod
            ? await _repository.GetUsersByRoleCodeAsync(RoleCodes.Hod, cancellationToken)
            : [];

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var currentHod in currentHods.Where(x => x.Id != user.Id))
            {
                currentHod.AssignRole(teacherRole!);
                currentHod.ClearSession();
            }

            user.AssignRole(role);
            user.ClearSession();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        if (normalizedRoleCode == RoleCodes.Hod)
        {
            await ReassignHODApproversAsync(currentHods, user.Id, cancellationToken);
        }

        return MapToUserDto(user);
    }

    public async Task<Guid> GetHODId(CancellationToken cancellationToken)
    {
        var result = await _repository.GetHODId(cancellationToken);

        return result;
    }

    public async Task<TeacherDto> GetHOD(CancellationToken cancellationToken)
    {
        var hodUsers = await _repository.GetUsersByRoleCodeAsync(RoleCodes.Hod, cancellationToken);
        var hod = hodUsers.FirstOrDefault()
            ?? throw new KeyNotFoundException("HOD user was not found.");

        return new TeacherDto(hod.Id, hod.Username);
    }

    public async Task<TeacherDto> AssignHOD(Guid newHodUserId, CancellationToken cancellationToken)
    {
        if (newHodUserId == Guid.Empty)
        {
            throw new ArgumentException("New HOD user id is required.", nameof(newHodUserId));
        }

        var newHod = await _repository.GetByIdWithRoleAsync(newHodUserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id {newHodUserId} was not found.");

        if (newHod.Role is null)
        {
            throw new InvalidOperationException("Selected user has no role.");
        }

        if (newHod.Role.RoleCode != RoleCodes.Teacher && newHod.Role.RoleCode != RoleCodes.Hod)
        {
            throw new InvalidOperationException("HOD can only be assigned to a teacher.");
        }

        var hodRole = await _repository.GetRoleByCodeAsync(RoleCodes.Hod, cancellationToken)
            ?? throw new InvalidOperationException("HOD role was not found.");
        var teacherRole = await _repository.GetRoleByCodeAsync(RoleCodes.Teacher, cancellationToken)
            ?? throw new InvalidOperationException("Teacher role was not found.");
        var currentHods = await _repository.GetUsersByRoleCodeAsync(RoleCodes.Hod, cancellationToken);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var currentHod in currentHods.Where(x => x.Id != newHod.Id))
            {
                currentHod.AssignRole(teacherRole);
                currentHod.ClearSession();
            }

            newHod.AssignRole(hodRole);
            newHod.ClearSession();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        await ReassignHODApproversAsync(currentHods, newHod.Id, cancellationToken);

        return new TeacherDto(newHod.Id, newHod.Username);
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
            user = await _repository.GetByUserNameAsync(email.Trim(), cancellationToken);
            
            if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash) || user.Role is null)
                return new LoginAttemptDto(false, null, "Invalid email or password.", false, false);
        }

        var verified = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verified == PasswordVerificationResult.Failed)
        {
            return new LoginAttemptDto(false, null, "Invalid email or password.", false, false);
        }

        if (user.Session.HasValue || user.SessionActiveOn.HasValue)
        {
            if (HasActiveSession(user, DateTime.UtcNow))
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

    public async Task<LoginAttemptDto> RefreshSessionAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            return new LoginAttemptDto(false, null, "Invalid user.", true, false);
        }

        var user = await _repository.GetByIdWithRoleAsync(userId, cancellationToken);
        if (user is null || user.Role is null)
        {
            return new LoginAttemptDto(false, null, "User was not found.", false, false);
        }

        if (!HasActiveSession(user, DateTime.UtcNow))
        {
            user.ClearSession();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new LoginAttemptDto(false, null, "Session has expired.", false, false);
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

    private static UserDto MapToUserDto(UserName user)
    {
        return new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.Role.RoleCode,
            user.Role.RoleName);
    }

    private bool HasActiveSession(UserName user, DateTime utcNow)
    {
        if (!user.Session.HasValue || !user.SessionActiveOn.HasValue)
        {
            return false;
        }

        var activeWindowMinutes = Math.Max(1, _jwtOptions.AccessTokenMinutes);
        var sessionStartedAt = DateTime.SpecifyKind(user.SessionActiveOn.Value, DateTimeKind.Utc);
        var sessionExpiredAt = sessionStartedAt.AddMinutes(activeWindowMinutes);

        // Let a login retry replace a session that is effectively at the timeout boundary.
        return sessionExpiredAt > utcNow.AddSeconds(5);
    }

    private async Task ReassignHODApproversAsync(
        IReadOnlyCollection<UserName> previousHods,
        Guid newHODApproverId,
        CancellationToken cancellationToken)
    {
        if (newHODApproverId == Guid.Empty)
        {
            return;
        }

        var previousHodIds = previousHods
            .Select(x => x.Id)
            .Where(x => x != Guid.Empty && x != newHODApproverId)
            .Distinct()
            .ToList();

        foreach (var previousHodId in previousHodIds)
        {
            await _reassignHODApproverClient.GetResponse<ReassignHODApproverCommandResponse>(
                new ReassignHODApproverCommand(previousHodId, newHODApproverId),
                cancellationToken);
        }
    }
}
