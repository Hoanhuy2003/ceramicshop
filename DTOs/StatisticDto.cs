using System.ComponentModel.DataAnnotations;

namespace ceramic.DTOs;

public class SalesQuantityRequest
{
   
    public DateTime From { get; set; }

    
    public DateTime To { get; set; }

    public string? Category { get; set; }
}

public class ProductRevenueRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    public DateTime From { get; set; }

    [Required]
    public DateTime To { get; set; }
}

public class SalesQuantityResponse
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProductsSold { get; set; }
    public List<ProductQuantityStat> ByProduct { get; set; } = new();
}

public class ProductQuantityStat
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
}

public class ProductRevenueResponse
{
    public string ProductName { get; set; } = string.Empty;
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<ProductRevenueByDate> ByDate { get; set; } = new();
}

public class ProductRevenueByDate
{
    public DateTime Date { get; set; }
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}