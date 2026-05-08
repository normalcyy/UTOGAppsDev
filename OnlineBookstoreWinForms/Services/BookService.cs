using OnlineBookstoreWinForms.Data.Interfaces;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Services;

public class BookService
{
    private readonly IBookRepository _books;

    public BookService(IBookRepository books) => _books = books;

    public Task<List<Book>> GetAllBooksAsync() => _books.GetAllAsync();

    public Task<List<Book>> SearchBooksAsync(string? title, string? authorName, int? categoryId, decimal? minPrice, decimal? maxPrice) =>
        _books.SearchAsync(title, authorName, categoryId, minPrice, maxPrice);

    public Task<Book?> GetBookByIdAsync(int id) => _books.GetByIdAsync(id);

    public Task<Book> AddBookAsync(Book book) => _books.AddAsync(book);

    public Task UpdateBookAsync(Book book) => _books.UpdateAsync(book);

    public async Task<(bool Success, string? Error)> DeleteBookAsync(int id)
    {
        if (await _books.HasOrderItemsAsync(id))
            return (false, "Cannot delete a book that has existing orders.");

        await _books.DeleteAsync(id);
        return (true, null);
    }
}
