using ceramic.DTOs;

namespace ceramic.Repositories
{
    public interface IStatisticsRepository
    {
        Task<SalesQuantityResponse> GetSalesQuantityAsync(DateTime from, DateTime to, string? category);
        Task<ProductRevenueResponse?> GetProductRevenueAsync(string productName, DateTime from, DateTime to);
    }
}
