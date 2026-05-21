using Auth.Authentication.Model;
using Auth.Dto;

namespace Auth.Service;

public interface IAuthService
{   
    Task<Guid> AddNewUser(UsernameDto user, CancellationToken cancellationToken);
    Task<LoginAttemptDto> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task<bool> LogoutAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<UserDto>> GetUsers(CancellationToken cancellationToken);
    Task<List<UserLookupDto>> GetUserLookup(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken);
    Task<UserDto> UpdateUserRole(Guid userId, string roleCode, CancellationToken cancellationToken);
    Task<Guid> GetHODId(CancellationToken cancellationToken);
    Task<TeacherDto> GetHOD(CancellationToken cancellationToken);
    Task<TeacherDto> AssignHOD(Guid newHodUserId, CancellationToken cancellationToken);
    Task<List<TeacherDto>> GetAllUserAsTeacher(CancellationToken cancellationToken);
}
