using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Data.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetByUserIdAsync(int userId);
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<Order> AddAsync(Order order);
    Task UpdateStatusAsync(int orderId, string newStatus);
}
