using ceramic.Models;
using System.Threading.Tasks;

namespace ceramic.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAll();
        Task<Product> GetById(int id);
        Task<Product> AddProduct(Product product);

        Task<Product> UpdateProduct(Product product);
        Task<bool> DeleteProduct(int id);
        Task<List<ProductPriceHistory>> GetPriceHistoryAsync(int productId);
        Task AddPriceHistoryAsync(ProductPriceHistory priceHistory);
        Task<List<string>> GetCategoriesAsync();




    }
}
