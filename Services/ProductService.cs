using ceramic.Models;
using ceramic.Repositories;
using ceramic.DTOs;

namespace ceramic.Services

{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<List<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAll();
            return products.Select(MapToProductResponse).ToList();

        }
        public async Task<ProductResponse?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetById(id);
            return product is null ? null : MapToProductResponse(product);  
        }
        public async Task<ProductResponse> CreateProductAsync(CreateProductDto product)
        {
            var newProduct = new Product
            {
                Name = product.Name.Trim(),
                Description = product.Description?.Trim(),
                Category = product.Category.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _productRepository.AddProduct(newProduct);

            await _productRepository.AddPriceHistoryAsync(new ProductPriceHistory
            {
                ProductId = newProduct.Id,
                Price = product.Price,
                EffectiveFrom = DateTime.UtcNow,
                EffectiveTo = null,
                Note = "Giá khởi tạo"
            });

            var createdProduct = await _productRepository.GetById(newProduct.Id);
            return MapToProductResponse(createdProduct);
        }
        public async Task<ProductResponse?> UpdateProductAsync(int id, UpdateProductDto product)
        {
            var existingProduct = await _productRepository.GetById(id);
            if (existingProduct is null) return null;

            existingProduct.Name = product.Name.Trim();
            existingProduct.Description = product.Description?.Trim();
            existingProduct.Category = product.Category.Trim();
            existingProduct.IsActive = true;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateProduct(existingProduct);
            return MapToProductResponse(existingProduct);
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteProduct(id);
        }

        public async Task<ProductResponse?> UpdatePriceAsync(int id, UpdateProductPriceDto priceDto)
        {
            var product = await _productRepository.GetById(id);
            if (product is null) return null;
            var currentPriceHistory = product.ProductPriceHistories
                .FirstOrDefault(p => p.EffectiveTo == null);
            if (currentPriceHistory != null)
            {
                currentPriceHistory.EffectiveTo = DateTime.UtcNow;
                await _productRepository.UpdateProduct(product);
            }
            await _productRepository.AddPriceHistoryAsync(new ProductPriceHistory
            {
                ProductId = product.Id,
                Price = priceDto.Price,
                EffectiveFrom = DateTime.UtcNow,
                EffectiveTo = null,
                Note = priceDto.Note?.Trim()
            });
            var updatedProduct = await _productRepository.GetById(id);
            return MapToProductResponse(updatedProduct);
        }

        public async Task<List<ProductPriceHistoryResponse>> GetPriceHistoryAsync(int id)
        {
            var history = await _productRepository.GetPriceHistoryAsync(id);
            return history.Select(h => new ProductPriceHistoryResponse
            {
                Id = h.Id,
                Price = h.Price,
                EffectiveFrom = h.EffectiveFrom,
                EffectiveTo = h.EffectiveTo,
                Note = h.Note
            }).ToList();

        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            return await _productRepository.GetCategoriesAsync();
        }

        private static ProductResponse MapToProductResponse(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.ProductPriceHistories
                .Where(p => p.EffectiveTo == null)
                .OrderByDescending(p => p.EffectiveFrom)
                .Select(p => p.Price)
                .FirstOrDefault(),
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
