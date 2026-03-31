using System.Reflection.Metadata.Ecma335;
using Shared.DDD;

namespace Auth.Authentication.Model;

public class UserName : Aggregate<Guid>
{
    public string Username { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public Guid? LaboratoryId {get; private set;} 
    public Guid? Session { get; private set; }
    public DateTime? SessionActiveOn { get; private set; }

    public UserRole Role { get; private set; } = default!;

    private UserName() { }

    private UserName(
        string username,
        string email
    )
    {
        Username = username;
        Email = email;
    }

    public static UserName Create(
        string username,
        string email
    )
    {
        return new UserName(
            username,
            email
        );
    }

    public void AssignRole(UserRole role)
    {
        ArgumentNullException.ThrowIfNull(role);
        Role = role;
    }

    public void SetPasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
    }

    public void SetSession()
    {
        Session = Guid.NewGuid();
        SessionActiveOn = DateTime.UtcNow;
    }

    public void ClearSession()
    {
        Session = null;
        SessionActiveOn = null;
    }

    public void AssignLaboratory(Guid LabId)
    {
        LaboratoryId = LabId;
    }
}
