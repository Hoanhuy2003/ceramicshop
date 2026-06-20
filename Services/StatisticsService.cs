using ceramic.DTOs;
using ceramic.Repositories;
using ceramic.Models;

namespace ceramic.Services
{
    public class StatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        public StatisticsService(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }
        public async Task<SalesQuantityResponse> GetSalesQuantityAsync(SalesQuantityRequest productSales)
        {
            return await _statisticsRepository.GetSalesQuantityAsync(productSales.From, productSales.To, productSales.Category);
        }
        public async Task<ProductRevenueResponse?> GetProductRevenueAsync(ProductRevenueRequest productRevenue)
        {
            return await _statisticsRepository.GetProductRevenueAsync(productRevenue.ProductName, productRevenue.From, productRevenue.To);
        }
    }
}
