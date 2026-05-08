using OnlineBookstoreWinForms.Data.Interfaces;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Data.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new List<Order>();
    private int _nextId = 1;
    private int _nextItemId = 1;

    public Task<List<Order>> GetByUserIdAsync(int userId) =>
        Task.FromResult(_orders.Where(o => o.UserId == userId).ToList());

    public Task<List<Order>> GetAllAsync() =>
        Task.FromResult(_orders.ToList());

    public Task<Order?> GetByIdAsync(int id) =>
        Task.FromResult(_orders.FirstOrDefault(o => o.Id == id));

    public Task<Order> AddAsync(Order order)
    {
        order.Id = _nextId++;
        foreach (var item in order.Items)
            item.Id = _nextItemId++;
        _orders.Add(order);
        return Task.FromResult(order);
    }

    public Task UpdateStatusAsync(int orderId, string newStatus)
    {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        if (order != null) order.Status = newStatus;
        return Task.CompletedTask;
    }
}
