using OnlineBookstoreWinForms.Data.Interfaces;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Data.Repositories;

public class InMemoryWishlistRepository : IWishlistRepository
{
    private readonly List<WishlistItem> _items = new List<WishlistItem>();
    private int _nextId = 1;

    public Task<List<WishlistItem>> GetByUserIdAsync(int userId) =>
        Task.FromResult(_items.Where(i => i.UserId == userId).ToList());

    public Task AddAsync(WishlistItem item)
    {
        item.Id = _nextId++;
        _items.Add(item);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(int userId, int bookId)
    {
        _items.RemoveAll(i => i.UserId == userId && i.BookId == bookId);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(int userId, int bookId) =>
        Task.FromResult(_items.Any(i => i.UserId == userId && i.BookId == bookId));

    public Task ClearForUserAsync(int userId)
    {
        _items.RemoveAll(i => i.UserId == userId);
        return Task.CompletedTask;
    }
}
