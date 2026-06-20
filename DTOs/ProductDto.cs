using System.ComponentModel.DataAnnotations;

namespace ceramic.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty!;
        public string? Description { get; set; }

        public string Category { get; set; } = string.Empty!;
        public decimal Price { get; set; }
    }

    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty!;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty!;
        public decimal Price { get; set; }
    }

    public class UpdateProductPriceDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá mới phải lớn hơn 0.")]
        public decimal Price { get; set; }

        public string? Note { get; set; }
    }

    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty!;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty!;
        public decimal Price { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ProductPriceHistoryResponse
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Note { get; set; }
    }
    

}
