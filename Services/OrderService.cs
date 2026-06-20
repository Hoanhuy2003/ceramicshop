using ceramic.DTOs;
using ceramic.Models;
using ceramic.Repositories;

namespace ceramic.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IStatisticsRepository statisticsRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;

        }

        public async Task<OrderResponse> Create(CreateOrderDto orderDto)
        {
            var now = DateTime.UtcNow;
            var details = new List<OrderDetail>();

            foreach (var detailDto in orderDto.OrderDetails)
            {
                var product = await _productRepository.GetById(detailDto.ProductId);
                if (product is null)
                {
                    throw new Exception($"Sản phẩm có {detailDto.ProductId} không tìm thấy.");
                }
                var priceHistory = product.ProductPriceHistories
                    .Where(ph => ph.EffectiveFrom <= now && (ph.EffectiveTo == null || ph.EffectiveTo >= now))
                    .OrderByDescending(ph => ph.EffectiveFrom)
                    .FirstOrDefault();
                if (priceHistory is null)
                {
                    throw new Exception($"Không tìm thấy lịch sử giá cho mã sản phẩm này {detailDto.ProductId}.");
                }
                details.Add(new OrderDetail
                {
                    ProductId = detailDto.ProductId,
                    Quantity = detailDto.Quantity,
                    UnitPrice = priceHistory.Price
                });
            }

            var order = new Order
            {
                OrderCode = await GenerateOrderCodeAsync(now),
                CustomerName = orderDto.CustomerName.Trim(),
                CustomerPhone = orderDto.CustomerPhone.Trim(),
                Note = orderDto.Note?.Trim(),
                OrderDate = now,
                TotalAmount = details.Sum(d => d.Quantity * d.UnitPrice),
                CreatedAt = now,
                OrderDetails = details

            };

            await _orderRepository.CreateAsync(order);

            var createdOrder = await _orderRepository.GetByIdAsync(order.Id);
            return MapToOrderResponse(createdOrder);
        }
        public async Task<List<OrderResponse>> GetAll(DateTime? from, DateTime? to)
        {
            var orders = await _orderRepository.GetAllAsync(from, to);
            return orders.Select(MapToOrderResponse).ToList();
        }

        public async Task<OrderResponse> GetById(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order is null)
            {
                throw new Exception($"Đơn hàng có id {id} không tìm thấy.");
            }
            return MapToOrderResponse(order);
        }

        public async Task<OrderResponse?> GetByCodeAsync(string orderCode)
        {
            var order = await _orderRepository.GetByCodeAsync(orderCode);
            return order is null ? null : MapToOrderResponse(order);
        }


        private async Task<string> GenerateOrderCodeAsync(DateTime now)
        {
            var count = await _orderRepository.CountTodayOrdersAsync(now);
            return $"ORD-{now:yyyyMMdd}-{(count + 1):D3}";
        }

       

        private static OrderResponse MapToOrderResponse(Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                Note = order.Note,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailResponse
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product.Name,
                    Category = od.Product.Category,
                    Quantity = od.Quantity,
                    unitPrice = od.UnitPrice,
                    Price = od.Quantity * od.UnitPrice
                }).ToList()
            };
        }


    }
}
