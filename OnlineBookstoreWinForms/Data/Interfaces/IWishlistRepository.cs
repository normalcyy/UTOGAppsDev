using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Data.Interfaces;

public interface IWishlistRepository
{
    Task<List<WishlistItem>> GetByUserIdAsync(int userId);
    Task AddAsync(WishlistItem item);
    Task RemoveAsync(int userId, int bookId);
    Task<bool> ExistsAsync(int userId, int bookId);
    Task ClearForUserAsync(int userId);
}
