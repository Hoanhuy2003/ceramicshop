using ceramic.DTOs;
using ceramic.Data;
using Microsoft.EntityFrameworkCore;

namespace ceramic.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly AppDbContext _context;

        public StatisticsRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ProductRevenueResponse?> GetProductRevenueAsync(string productName, DateTime from, DateTime to)
        {
            var toEnd = to.AddDays(1).AddTicks(-1);
            var keyword = productName.Trim().ToLower();

            var product = await _context.Products
                .Where(p => p.Name.ToLower().Contains(keyword))
                .FirstOrDefaultAsync();
            if (product == null) return null;

            var orderDetails = await _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.ProductId == product.Id 
                && od.Order.OrderDate >= from 
                && od.Order.OrderDate <= toEnd)
                .ToListAsync();
            var byDate = orderDetails
                .GroupBy(od => od.Order.OrderDate.Date)
                .Select(g => new ProductRevenueByDate
                {
                    Date = g.Key,
                    QuantitySold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.Quantity * od.UnitPrice)
                })
                .OrderBy(d => d.Date)
                .ToList();
            return new ProductRevenueResponse
            {
                ProductName = product.Name,
                From = from,
                To = to,
                TotalQuantitySold = byDate.Sum(d => d.QuantitySold),
                TotalRevenue = byDate.Sum(d => d.Revenue),
                ByDate = byDate
            };
        }

        public async Task<SalesQuantityResponse> GetSalesQuantityAsync(DateTime from, DateTime to, string? category)
        {
            var toEnd = to.AddDays(1).AddTicks(-1);

            var query = _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .Where(od => od.Order.OrderDate >= from && od.Order.OrderDate <= toEnd);

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(od => od.Product.Category == category);
            }

            var stats = await query
                .GroupBy(od => new{od.ProductId, od.Product.Name, od.Product.Category})
                .Select(g => new ProductQuantityStat
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    Category = g.Key.Category,
                    QuantitySold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(s => s.QuantitySold)
                .ToListAsync();

            var totalOrders = await _context.Orders
                .CountAsync(o => o.OrderDate >= from && o.OrderDate <= toEnd);

            return new SalesQuantityResponse
            {
                From = from,
                To = to,
                TotalOrders = totalOrders,
                TotalProductsSold = stats.Sum(s => s.QuantitySold),
                ByProduct = stats
            };



        }
    }
}
