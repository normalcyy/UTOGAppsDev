using OnlineBookstoreWinForms.Data.Interfaces;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Services;

public class WishlistService
{
    private readonly IWishlistRepository _wishlist;
    private readonly IBookRepository     _books;

    public WishlistService(IWishlistRepository wishlist, IBookRepository books)
    {
        _wishlist = wishlist;
        _books    = books;
    }

    public Task<List<WishlistItem>> GetWishlistAsync(int userId) =>
        _wishlist.GetByUserIdAsync(userId);

    public async Task<(bool Success, string? Error)> AddToWishlistAsync(int userId, int bookId)
    {
        if (await _wishlist.ExistsAsync(userId, bookId))
            return (false, "Book is already in your wishlist.");

        var book = await _books.GetByIdAsync(bookId);
        if (book == null)
            return (false, "Book not found.");

        await _wishlist.AddAsync(new WishlistItem
        {
            UserId    = userId,
            BookId    = bookId,
            BookTitle = book.Title,
            BookPrice = book.Price,
            AddedAt   = DateTime.UtcNow,
        });
        return (true, null);
    }

    public Task RemoveFromWishlistAsync(int userId, int bookId) =>
        _wishlist.RemoveAsync(userId, bookId);

    public Task<bool> IsInWishlistAsync(int userId, int bookId) =>
        _wishlist.ExistsAsync(userId, bookId);
}
