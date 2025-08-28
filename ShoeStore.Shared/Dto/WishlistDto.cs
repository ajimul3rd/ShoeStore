using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoeStore.Shared.Dto
{
    public class WishlistDto
    {
        public int? WishlistId { get; set; }

        [Required]
        [ForeignKey("Users")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("ProductVariant")]
        public int VariantsId { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [JsonIgnore]
        public UsersDto? UsersDto { get; set; }
        public ProductVariantDto? ProductVariantDto { get; set; }
    }
}
