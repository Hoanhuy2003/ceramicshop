using ceramic.Models;
using ceramic.Data;
using Microsoft.EntityFrameworkCore;

namespace ceramic.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> CountTodayOrdersAsync(DateTime date)
        {
            return await _context.Orders.CountAsync(o => o.OrderDate.Date == date.Date);
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetAllAsync(DateTime? from, DateTime? to)
        {
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .AsQueryable();

            if (from.HasValue)
                query = query.Where(o => o.OrderDate >= from.Value);
            if (to.HasValue)
                query = query.Where(o => o.OrderDate <= to.Value);
            return await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public Task<Order?> GetByCodeAsync(string orderCode)
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }

        public Task<Order?> GetByIdAsync(int id)
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
