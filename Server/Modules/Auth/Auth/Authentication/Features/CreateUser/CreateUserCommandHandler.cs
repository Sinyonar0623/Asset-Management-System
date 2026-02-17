using Auth.Dto;
using Auth.Service;
using Mapster;
using Shared.CQRS;

namespace Auth.Authentication.Features.CreateUser;

public sealed class CreateUserCommandHandler(IAuthService authService)
    : ICommandHandler<CreateUserCommand, CreateUserResult>
{

    private readonly IAuthService _service = authService;

    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Password)
            || string.IsNullOrWhiteSpace(request.RoleCode))
        {
            return new CreateUserResult(false, null, "Username, email, password and roleCode are required.");
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
        //     return new CreateUserResult(false, null, "Username or email already exists.");
        // }

        // var role = await dbContext.Set<UserRole>()
        //     .FirstOrDefaultAsync(x => x.RoleCode == roleCode, cancellationToken);

        // if (role is null)
        // {
        //     return new CreateUserResult(false, null, "Role not found.");
        // }

        // var user = UserName.Create(username, email, string.Empty);
        // var passwordHash = passwordHasher.HashPassword(user, request.Password);
        // user.SetPasswordHash(passwordHash);
        // user.AssignRole(role);

        // await dbContext.Auth.AddAsync(user, cancellationToken);
        // await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateUserResult(true, result, null);
    }
}
