using System.Reflection.Metadata.Ecma335;
using Shared.DDD;

namespace Auth.Authentication.Model;

public class UserName : Aggregate<Guid>
{
    public string Username { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public Guid? Session { get; private set; }
    public DateTime? SessionActiveOn { get; private set; }

    public UserRole Role { get; private set; } = default!;

    private UserName() { }

    private UserName(
        string username,
        string email,
        string pass
    )
    {
        Username = username;
        Email = email;
        PasswordHash = pass;
    }

    public static UserName Create(
        string username,
        string email,
        string pass
    )
    {
        return new UserName(
            username,
            email,
            pass
        );
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
}