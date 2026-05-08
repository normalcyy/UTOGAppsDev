using OnlineBookstoreWinForms.Data.Interfaces;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Services;

public class AuthService
{
    private readonly IUserRepository _users;

    public AuthService(IUserRepository users) => _users = users;

    public async Task<(bool Success, string? Error)> RegisterAsync(string username, string password, string email)
    {
        if (string.IsNullOrWhiteSpace(username))
            return (false, "Username is required.");
        if (password.Length < 8)
            return (false, "Password must be at least 8 characters.");
        if (!email.Contains('@'))
            return (false, "Email must contain '@'.");

        if (await _users.UsernameExistsAsync(username))
            return (false, "Username is already taken.");

        var user = new User
        {
            Username     = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Email        = email,
            Role         = Constants.Roles.User,
            CreatedAt    = DateTime.UtcNow,
        };

        await _users.AddAsync(user);
        return (true, null);
    }

    public async Task<(User? User, string? Error)> LoginAsync(string username, string password)
    {
        var user = await _users.GetByUsernameAsync(username);
        if (user == null)
            return (null, "Invalid username or password.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (null, "Invalid username or password.");

        return (user, null);
    }
}
