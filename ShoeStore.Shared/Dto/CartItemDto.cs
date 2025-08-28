using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoeStore.Shared.Dto
{
    public class CartItemDto
    {
        public int? CartItemId { get; set; }

        [Required]
        [ForeignKey("ShoppingCart")]
        public int CartId { get; set; }

        [Required]
        [ForeignKey("ProductVariant")]
        public int VariantsId { get; set; }

        [Required]
        [ForeignKey("SizeVariant")]
        public int SizesId { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        public bool IsSavedForLater { get; set; } = false;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [JsonIgnore]
        public ShoppingCartDto? ShoppingCartDto { get; set; }
        public ProductVariantDto? ProductVariantDto { get; set; }
        public SizeVariantDto? SizeVariantDto { get; set; }
    }
}