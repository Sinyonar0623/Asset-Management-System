using Auth.Dto;
using Auth.Service;
using Mapster;
using Shared.CQRS;

namespace Auth.Authentication.Features.SignUp;

public sealed class SignUpCommandHandler(IAuthService authService)
    : ICommandHandler<SignUpCommand, SignUpResult>
{

    private readonly IAuthService _service = authService;

    public async Task<SignUpResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Password)
            || string.IsNullOrWhiteSpace(request.RoleCode))
        {
            return new SignUpResult(false, null, "Username, email, password and roleCode are required.");
        }

        var user = request.Adapt<UsernameDto>();

        var result = await _service.AddNewUser(user, cancellationToken);

        // var username = request.Username.Trim();
        // var email = request.Email.Trim();
        // var roleCode = request.RoleCode.Trim();

        // var userExists = await dbContext.Auth
        //     .AsNoTracking()
        //     .AnyAsync(x => x.Username == username || x.Email == email, cancellationToken);

        // if (userExists)
        // {
        //     return new SignUpResult(false, null, "Username or email already exists.");
        // }

        // var role = await dbContext.Set<UserRole>()
        //     .FirstOrDefaultAsync(x => x.RoleCode == roleCode, cancellationToken);

        // if (role is null)
        // {
        //     return new SignUpResult(false, null, "Role not found.");
        // }

        // var user = UserName.Create(username, email, string.Empty);
        // var passwordHash = passwordHasher.HashPassword(user, request.Password);
        // user.SetPasswordHash(passwordHash);
        // user.AssignRole(role);

        // await dbContext.Auth.AddAsync(user, cancellationToken);
        // await dbContext.SaveChangesAsync(cancellationToken);

        return new SignUpResult(true, result, null);
    }
}
