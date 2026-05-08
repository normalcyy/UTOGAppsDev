using OnlineBookstoreWinForms.Data.Interfaces;
using OnlineBookstoreWinForms.Data.Repositories;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Services;

public class OrderService
{
    private readonly IOrderRepository    _orders;
    private readonly IWishlistRepository _wishlist;
    private readonly IBookRepository     _books;

    public OrderService(IOrderRepository orders, IWishlistRepository wishlist, IBookRepository books)
    {
        _orders   = orders;
        _wishlist = wishlist;
        _books    = books;
    }

    public async Task<Order> PlaceOrderAsync(int userId, List<WishlistItem> wishlistItems)
    {
        var items = wishlistItems.Select(wi => new OrderItem
        {
            BookId    = wi.BookId,
            BookTitle = wi.BookTitle,
            Quantity  = 1,
            UnitPrice = wi.BookPrice,
        }).ToList();

        var order = new Order
        {
            UserId      = userId,
            OrderDate   = DateTime.UtcNow,
            Status      = Constants.OrderStatus.Pending,
            TotalAmount = items.Sum(i => i.UnitPrice * i.Quantity),
            Items       = items,
        };

        await _orders.AddAsync(order);

        if (_books is InMemoryBookRepository repo)
            repo.RegisterOrderItems(order.Items);

        await _wishlist.ClearForUserAsync(userId);
        return order;
    }

    public Task<List<Order>> GetOrderHistoryAsync(int userId) =>
        _orders.GetByUserIdAsync(userId);

    public Task<List<Order>> GetAllOrdersAsync() =>
        _orders.GetAllAsync();

    public Task UpdateOrderStatusAsync(int orderId, string newStatus) =>
        _orders.UpdateStatusAsync(orderId, newStatus);
}
