using ceramic.Models;

namespace ceramic.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(DateTime? from, DateTime? to);
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetByCodeAsync(string orderCode);
    Task<Order> CreateAsync(Order order);
    Task<int> CountTodayOrdersAsync(DateTime date);
}

