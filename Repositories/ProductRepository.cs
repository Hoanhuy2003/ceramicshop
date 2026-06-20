using ceramic.Models;
using ceramic.Data;
using Microsoft.EntityFrameworkCore;

namespace ceramic.Repositories
{
    public class ProductRepository : IProductRepository
    {

        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddPriceHistoryAsync(ProductPriceHistory priceHistory)
        {
            _context.ProductPriceHistories.Add(priceHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<Product> AddProduct(Product product)
        {
            _context.Products.Add(product);

            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null) return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Product>> GetAll()
        {
            return await _context.Products
                .Include(p => p.ProductPriceHistories)
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product> GetById(int id)
        {
            return await _context.Products
                .Include(p => p.ProductPriceHistories)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive == true)
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<List<ProductPriceHistory>> GetPriceHistoryAsync(int productId)
        {
            return await _context.ProductPriceHistories
                .Where(ph => ph.ProductId == productId)
                .OrderByDescending(ph => ph.EffectiveFrom)
                .ToListAsync();
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
