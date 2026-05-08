using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Data.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> UsernameExistsAsync(string username);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
}
