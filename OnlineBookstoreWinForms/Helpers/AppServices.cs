using OnlineBookstoreWinForms.Data.Repositories;
using OnlineBookstoreWinForms.Services;

namespace OnlineBookstoreWinForms.Helpers;

/// <summary>
/// Singleton service locator — one shared instance of every repository and service.
/// </summary>
public static class AppServices
{
    public static readonly InMemoryAuthorRepository   AuthorRepo   = new InMemoryAuthorRepository();
    public static readonly InMemoryBookRepository     BookRepo     = new InMemoryBookRepository();
    public static readonly InMemoryCategoryRepository CategoryRepo = new InMemoryCategoryRepository();
    public static readonly InMemoryOrderRepository    OrderRepo    = new InMemoryOrderRepository();
    public static readonly InMemoryUserRepository     UserRepo     = new InMemoryUserRepository();
    public static readonly InMemoryWishlistRepository WishlistRepo = new InMemoryWishlistRepository();

    public static readonly AuthService     Auth     = new AuthService(UserRepo);
    public static readonly BookService     Books    = new BookService(BookRepo);
    public static readonly OrderService    Orders   = new OrderService(OrderRepo, WishlistRepo, BookRepo);
    public static readonly WishlistService Wishlist = new WishlistService(WishlistRepo, BookRepo);
}
