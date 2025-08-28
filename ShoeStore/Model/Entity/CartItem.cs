using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoeStore.Model.Entity
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemId { get; set; }

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
        public ShoppingCart? ShoppingCart { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public SizeVariant? SizeVariant { get; set; }
    }
}