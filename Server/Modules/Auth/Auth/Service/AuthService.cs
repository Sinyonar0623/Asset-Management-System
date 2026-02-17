using Auth.Authentication.Model;
using Auth.Data.Repository;
using Auth.Data.UnitOfWork;
using Auth.Dto;
using Microsoft.AspNetCore.Identity;

namespace Auth.Service;

public class AuthService(
    IAuthRepository authRepository,
    IAuthUnitOfWork unitOfWork,
    IPasswordHasher<UserName> passwordHasher) : IAuthService
{
    private readonly IPasswordHasher<UserName> _hasher = passwordHasher;
    private readonly IAuthRepository _repository = authRepository;
    private readonly IAuthUnitOfWork _unitOfWork = unitOfWork;

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
}
