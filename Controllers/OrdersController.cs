using ceramic.DTOs;
using ceramic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace ceramic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> create([FromBody] CreateOrderDto order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
            var createdOrder = await _orderService.Create(order);
            return CreatedAtAction(nameof(getById), new { id = createdOrder.Id }, ApiResponse<OrderResponse>.Ok(createdOrder));
        }
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getById(int id)
        {
            var order = await _orderService.GetById(id);
            if (order is null) return NotFound(ApiResponse<OrderResponse>.Fail("Không tìm thấy đơn hàng"));
            return Ok(ApiResponse<OrderResponse>.Ok(order));
        }
        [HttpGet("code/{orderCode}")]
        [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getByCode(string orderCode)
        {
            var order = await _orderService.GetByCodeAsync(orderCode);
            if (order is null) return NotFound(ApiResponse<OrderResponse>.Fail("Không tìm thấy đơn hàng"));
            return Ok(ApiResponse<OrderResponse>.Ok(order));

        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<OrderResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> getAll(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null
            )
        {
            var orders = await _orderService.GetAll(from, to);
            return Ok(ApiResponse<List<OrderResponse>>.Ok(orders));
        }
    }
 }
