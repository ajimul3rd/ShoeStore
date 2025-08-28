using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoeStore.Shared.Dto
{
    public class SizeVariantDto
    {
        public int? SizesId { get; set; }

        [Required]
        [ForeignKey("ProductVariant")]
        public int VariantsId { get; set; }

        [Required]
        [MaxLength(10)]
        public string? Size { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MRP { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        // Navigation property
        [JsonIgnore]
        public ProductVariantDto? ProductVariantDto { get; set; }
    }
}