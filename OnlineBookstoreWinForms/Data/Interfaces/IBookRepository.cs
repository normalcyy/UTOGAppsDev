using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Data.Interfaces;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task<List<Book>> SearchAsync(string? title, string? authorName, int? categoryId, decimal? minPrice, decimal? maxPrice);
    Task<Book> AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(int id);
    Task<bool> HasOrderItemsAsync(int bookId);
}
