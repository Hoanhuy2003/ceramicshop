using ceramic.Services;
using ceramic.DTOs;
using Microsoft.AspNetCore.Mvc;
namespace ceramic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsService _statisticsService;

        public StatisticsController(StatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("sale")]
        [ProducesResponseType(typeof(ApiResponse<SalesQuantityResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSalesQuantity([FromQuery] SalesQuantityRequest request)
        {
            var result = await _statisticsService.GetSalesQuantityAsync(request);
            return Ok(ApiResponse<SalesQuantityResponse>.Ok(result));
        }

        [HttpGet("product-revenue")]
        [ProducesResponseType(typeof(ApiResponse<ProductRevenueResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductRevenue([FromQuery] ProductRevenueRequest request)
        {

            if (request.From > request.To)
            {
                return BadRequest(ApiResponse<object>.Fail("Ngày bắt đầu phải nhỏ hơn ngày kết thúc"));
            }
            var result = await _statisticsService.GetProductRevenueAsync(request);
            if (result is null) return NotFound(ApiResponse<ProductRevenueResponse>.Fail("Không tìm thấy sản phẩm"));
            return Ok(ApiResponse<ProductRevenueResponse>.Ok(result));

        }
    }
}
