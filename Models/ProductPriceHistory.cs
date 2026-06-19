using System;
using System.Collections.Generic;

namespace ceramic.Models
{
    public partial class ProductPriceHistory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Note { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}
