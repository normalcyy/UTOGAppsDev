using OnlineBookstoreWinForms.Data.Interfaces;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Data.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new List<User>();
    private int _nextId = 1;

    public InMemoryUserRepository()
    {
        _users.Add(new User
        {
            Id           = _nextId++,
            Username     = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Email        = "admin@bookstore.com",
            Role         = Constants.Roles.Admin,
            CreatedAt    = DateTime.UtcNow,
        });
        _users.Add(new User
        {
            Id           = _nextId++,
            Username     = "user1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
            Email        = "user1@bookstore.com",
            Role         = Constants.Roles.User,
            CreatedAt    = DateTime.UtcNow,
        });
    }

    public Task<User?> GetByIdAsync(int id) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

    public Task<User?> GetByUsernameAsync(string username) =>
        Task.FromResult(_users.FirstOrDefault(u =>
            string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase)));

    public Task<bool> UsernameExistsAsync(string username) =>
        Task.FromResult(_users.Any(u =>
            string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase)));

    public Task<User> AddAsync(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        var idx = _users.FindIndex(u => u.Id == user.Id);
        if (idx >= 0) _users[idx] = user;
        return Task.CompletedTask;
    }
}
