using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoeStore.Shared.Dto
{
    public class OrderItemDto
    {
        public int? OrderItemId { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("ProductVariant")]
        public int VariantsId { get; set; }

        [Required]
        [ForeignKey("SizeVariant")]
        public int SizesId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // Snapshot of product details at time of purchase
        [MaxLength(200)]
        public string? ProductName { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }

        [MaxLength(10)]
        public string? Size { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        // Navigation properties
        [JsonIgnore]
        public OrderDto? Order { get; set; }
        public ProductVariantDto? ProductVariantDto { get; set; }
        public SizeVariantDto? SizeVariantDto { get; set; }
    }
}