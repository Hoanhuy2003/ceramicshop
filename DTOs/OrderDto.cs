namespace ceramic.DTOs
{
    public class CreateOrderDto
    {
        public string CustomerName { get; set; } = string.Empty!;
        public string CustomerPhone { get; set; } = string.Empty!;
        public string? Note { get; set; }
        public List<CreateOrderDetailDto> OrderDetails { get; set; } = new List<CreateOrderDetailDto>();
    }
    public class CreateOrderDetailDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderResponse
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty!;
        public string CustomerPhone { get; set; } = string.Empty!;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal TotalAmount { get; set; }
        public List<OrderDetailResponse> OrderDetails { get; set; } = new List<OrderDetailResponse>();
    }

    public class OrderDetailResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty!;

        public string Category { get; set; } = string.Empty!;
        public int Quantity { get; set; }
        public decimal unitPrice { get; set; }
        public decimal Price { get; set; }
    }
}
