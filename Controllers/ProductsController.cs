using Microsoft.AspNetCore.Mvc;
using ceramic.Services;
using ceramic.DTOs;

namespace ceramic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProductsController : ControllerBase

    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ProductResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> getAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(ApiResponse<List<ProductResponse>>.Ok(products));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null) return NotFound(ApiResponse<ProductResponse>.Fail("Không tìm thấy sản phẩm"));
            return Ok(ApiResponse<ProductResponse>.Ok(product));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> create([FromBody] CreateProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var createdProduct = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(getById), new { id = createdProduct.Id }, ApiResponse<ProductResponse>.Ok(createdProduct));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> update(int id, [FromBody] UpdateProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var updatedProduct = await _productService.UpdateProductAsync(id, product);
            if (updatedProduct is null) return NotFound(ApiResponse<ProductResponse>.Fail("Không tìm thấy sản phẩm"));
            return Ok(ApiResponse<ProductResponse>.Ok(updatedProduct));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound(ApiResponse<object>.Fail("Không tìm thấy sản phẩm"));
            return Ok(ApiResponse<object>.Ok(null, "Xóa sản phẩm thành công"));
        }

        [HttpPatch("{id:int}/price")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> updatePrice(int id, [FromBody] UpdateProductPriceDto priceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var result = await _productService.UpdatePriceAsync(id, priceDto);
            if (result is null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy sản phẩm"));
            return Ok(ApiResponse<object>.Ok(null, "Cập nhật giá sản phẩm thành công"));
        }

        [HttpGet("{id:int}/price-history")]
        [ProducesResponseType(typeof(ApiResponse<List<ProductPriceHistoryResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
       
        public async Task<IActionResult> getPriceHistory(int id)
        {
            var priceHistory = await _productService.GetPriceHistoryAsync(id);
            if (priceHistory is null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy sản phẩm"));
            return Ok(ApiResponse<List<ProductPriceHistoryResponse>>.Ok(priceHistory));
        }

        [HttpGet("categories")]
        [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]

        public async Task<IActionResult> getCategories()
        {
            var categories = await _productService.GetCategoriesAsync();
            return Ok(ApiResponse<List<string>>.Ok(categories));
        }


    }
}
