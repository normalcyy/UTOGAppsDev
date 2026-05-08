using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Data.Interfaces;

public interface IAuthorRepository
{
    Task<List<Author>> GetAllAsync();
    Task<Author?> GetByIdAsync(int id);
    Task<Author> AddAsync(Author author);
    Task UpdateAsync(Author author);
    Task DeleteAsync(int id);
}
